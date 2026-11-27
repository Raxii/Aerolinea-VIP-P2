
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace MoldeMVC_Core.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            //************** NOMBRE DE USUARIO **********************
            [Required]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }
            //********************************************************

            [Required]
            [EmailAddress]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; }
        }

        //************** INICIO DE LOGIN FEDERADO *******************
        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(
            string provider,
            string returnUrl = null)
        {
            var redirectUrl = Url.Page(
                "./ExternalLogin",
                pageHandler: "Callback",
                values: new { returnUrl });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(
                    provider,
                    redirectUrl);

            return new ChallengeResult(provider, properties);
        }
        //***********************************************************

        //************** RESPUESTA DE GOOGLE O MICROSOFT ************
        public async Task<IActionResult> OnGetCallbackAsync(
            string returnUrl = null,
            string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage =
                    $"Error from external provider: {remoteError}";

                return RedirectToPage(
                    "./Login",
                    new { ReturnUrl = returnUrl });
            }

            var info =
                await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage =
                    "Error loading external login information.";

                return RedirectToPage(
                    "./Login",
                    new { ReturnUrl = returnUrl });
            }

            // Intentar iniciar sesión si ya tiene una cuenta vinculada
            var result =
                await _signInManager.ExternalLoginSignInAsync(
                    info.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "{Name} logged in with {LoginProvider} provider.",
                    info.Principal.Identity.Name,
                    info.LoginProvider);

                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            // Si es un usuario nuevo, mostrar el formulario
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;

            // Obtener nombre y correo del proveedor
            var email =
                info.Principal.FindFirstValue(ClaimTypes.Email);

            var nombre =
                info.Principal.FindFirstValue(ClaimTypes.Name);

            Input = new InputModel
            {
                Email = email,
                UserName = !string.IsNullOrWhiteSpace(nombre)
                    ? nombre
                    : email?.Split('@')[0]
            };

            return Page();
        }
        //***********************************************************

        //************** REGISTRO MEDIANTE GOOGLE O MICROSOFT ********
        public async Task<IActionResult> OnPostConfirmationAsync(
            string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info =
                await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage =
                    "Error loading external login information during confirmation.";

                return RedirectToPage(
                    "./Login",
                    new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // Guardar nombre de usuario personalizado
                await _userStore.SetUserNameAsync(
                    user,
                    Input.UserName,
                    CancellationToken.None);

                // Guardar correo electrónico
                await _emailStore.SetEmailAsync(
                    user,
                    Input.Email,
                    CancellationToken.None);

                // Crear usuario en ASP.NET Core Identity
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    // Vincular la cuenta con Google o Microsoft
                    result = await _userManager.AddLoginAsync(
                        user,
                        info);

                    if (result.Succeeded)
                    {
                        //************************************************
                        // ASIGNAR AUTOMATICAMENTE EL ROL PASAJERO
                        //************************************************
                        var resultadoRol =
                            await _userManager.AddToRoleAsync(
                                user,
                                "Pasajero");

                        if (!resultadoRol.Succeeded)
                        {
                            // Evitar dejar la nueva cuenta sin rol
                            await _userManager.DeleteAsync(user);

                            foreach (var error in resultadoRol.Errors)
                            {
                                ModelState.AddModelError(
                                    string.Empty,
                                    error.Description);
                            }

                            ProviderDisplayName =
                                info.ProviderDisplayName;

                            ReturnUrl = returnUrl;

                            return Page();
                        }
                        //************************************************

                        _logger.LogInformation(
                            "User created an account using {Name} provider.",
                            info.LoginProvider);

                        var userId =
                            await _userManager.GetUserIdAsync(user);

                        var code =
                            await _userManager
                                .GenerateEmailConfirmationTokenAsync(user);

                        code = WebEncoders.Base64UrlEncode(
                            Encoding.UTF8.GetBytes(code));

                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new
                            {
                                area = "Identity",
                                userId = userId,
                                code = code
                            },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(
                            Input.Email,
                            "Confirm your email",
                            $"Please confirm your account by " +
                            $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                            $"clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage(
                                "./RegisterConfirmation",
                                new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(
                            user,
                            isPersistent: false,
                            info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }

                    // Si no se pudo vincular el proveedor,
                    // evitar dejar una cuenta incompleta.
                    await _userManager.DeleteAsync(user);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;

            return Page();
        }
        //***********************************************************

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class " +
                    "and has a parameterless constructor, or alternatively " +
                    "override the external login page in " +
                    "/Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
