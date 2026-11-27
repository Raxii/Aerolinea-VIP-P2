using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoldeMVC_Core.Controllers
{
    // MOD-11: gestión de roles con ASP.NET Core Identity.
    // No modifica las conexiones, ni usa ViewModels ni DTOs.
    [Authorize(Roles = "Superusuario,Administrador")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        private static readonly string[] RolesIniciales =
        {
            "Superusuario", "Administrador", "Counter", "Pasajero"
        };

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        private bool EsSuperusuario() => User.IsInRole("Superusuario");

        private static bool EsRolInicial(string? nombre) =>
            RolesIniciales.Any(r => string.Equals(r, nombre,
                StringComparison.OrdinalIgnoreCase));

        private static bool EsRolPrivilegiado(string? nombre) =>
            string.Equals(nombre, "Superusuario", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(nombre, "Administrador", StringComparison.OrdinalIgnoreCase);

        private async Task<bool> PuedeAdministrarUsuarioAsync(IdentityUser usuario)
        {
            if (EsSuperusuario()) return true;
            var roles = await _userManager.GetRolesAsync(usuario);
            return !roles.Any(EsRolPrivilegiado);
        }

        private static bool NombreValido(string? nombre) =>
            !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length <= 50;

        // LISTAR ROLES
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name).ToListAsync();
            return View(roles);
        }

        // DETALLES DEL ROL
        public async Task<IActionResult> Details(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();
            ViewBag.Usuarios = await _userManager.GetUsersInRoleAsync(rol.Name!);
            return View(rol);
        }

        // CREAR ROL: SOLO SUPERUSUARIO
        [Authorize(Roles = "Superusuario")]
        [HttpGet]
        public IActionResult Create() => View(new IdentityRole());

        [Authorize(Roles = "Superusuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? nombre)
        {
            nombre = nombre?.Trim();
            if (!NombreValido(nombre))
                ModelState.AddModelError("nombre", "Ingrese un nombre de rol de hasta 50 caracteres.");
            if (ModelState.IsValid && await _roleManager.RoleExistsAsync(nombre!))
                ModelState.AddModelError("nombre", "Ya existe un rol con ese nombre.");

            if (!ModelState.IsValid)
            {
                ViewBag.Nombre = nombre;
                return View(new IdentityRole());
            }

            var resultado = await _roleManager.CreateAsync(new IdentityRole(nombre!));
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                ViewBag.Nombre = nombre;
                return View(new IdentityRole());
            }

            TempData["Mensaje"] = "Rol creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // EDITAR NOMBRE: SOLO ROLES ADICIONALES, NO LOS 4 INICIALES
        [Authorize(Roles = "Superusuario")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();
            if (EsRolInicial(rol.Name)) return Forbid();
            return View(rol);
        }

        [Authorize(Roles = "Superusuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string? nombre)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();
            if (EsRolInicial(rol.Name)) return Forbid();

            nombre = nombre?.Trim();
            if (!NombreValido(nombre))
                ModelState.AddModelError("nombre", "Ingrese un nombre de rol de hasta 50 caracteres.");

            if (ModelState.IsValid && !string.Equals(rol.Name, nombre,
                StringComparison.OrdinalIgnoreCase) && await _roleManager.RoleExistsAsync(nombre!))
                ModelState.AddModelError("nombre", "Ya existe un rol con ese nombre.");

            if (!ModelState.IsValid)
            {
                ViewBag.Nombre = nombre;
                return View(rol);
            }

            rol.Name = nombre;
            var resultado = await _roleManager.UpdateAsync(rol);
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                ViewBag.Nombre = nombre;
                return View(rol);
            }

            TempData["Mensaje"] = "Rol actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR ROL: SOLO ROLES ADICIONALES SIN USUARIOS
        [Authorize(Roles = "Superusuario")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();
            if (EsRolInicial(rol.Name)) return Forbid();
            ViewBag.CantidadUsuarios = (await _userManager.GetUsersInRoleAsync(rol.Name!)).Count;
            return View(rol);
        }

        [Authorize(Roles = "Superusuario")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();
            if (EsRolInicial(rol.Name)) return Forbid();

            if ((await _userManager.GetUsersInRoleAsync(rol.Name!)).Count > 0)
            {
                TempData["Error"] = "No se puede eliminar un rol con usuarios asignados.";
                return RedirectToAction(nameof(Index));
            }

            var resultado = await _roleManager.DeleteAsync(rol);
            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join("; ",
                    resultado.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Rol eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // LISTAR USUARIOS Y SUS ROLES
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _userManager.Users
                .OrderBy(u => u.UserName).ToListAsync();
            var asignaciones = new Dictionary<string, string>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                asignaciones[usuario.Id] = string.Join(", ", roles);
            }

            ViewBag.RolesUsuarios = asignaciones;
            return View(usuarios);
        }

        private async Task<IActionResult> MostrarAsignacionAsync(IdentityUser usuario)
        {
            ViewBag.RolesActuales = await _userManager.GetRolesAsync(usuario);
            var todos = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.RolesDisponibles = EsSuperusuario()
                ? todos
                : todos.Where(r => string.Equals(r.Name, "Counter", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r.Name, "Pasajero", StringComparison.OrdinalIgnoreCase)).ToList();
            return View("Asignar", usuario);
        }

        // ASIGNAR ROLES A UN USUARIO EXISTENTE
        [HttpGet]
        public async Task<IActionResult> Asignar(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();
            if (!await PuedeAdministrarUsuarioAsync(usuario)) return Forbid();
            return await MostrarAsignacionAsync(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(string id, string? rol)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();
            if (!await PuedeAdministrarUsuarioAsync(usuario)) return Forbid();

            if (!EsSuperusuario() &&
                !string.Equals(rol, "Counter", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(rol, "Pasajero", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var rolExistente = string.IsNullOrWhiteSpace(rol)
                ? null : await _roleManager.FindByNameAsync(rol);
            if (rolExistente == null)
            {
                ModelState.AddModelError("rol", "Seleccione un rol válido.");
                return await MostrarAsignacionAsync(usuario);
            }

            if (!await _userManager.IsInRoleAsync(usuario, rolExistente.Name!))
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, rolExistente.Name!);
                if (!resultado.Succeeded)
                {
                    foreach (var error in resultado.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return await MostrarAsignacionAsync(usuario);
                }
            }

            TempData["Mensaje"] = "Rol asignado correctamente.";
            return RedirectToAction(nameof(Asignar), new { id });
        }

        // QUITAR ROL A UN USUARIO, CON PROTECCIÓN AL SUPERUSUARIO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Quitar(string id, string? rol)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();
            if (!await PuedeAdministrarUsuarioAsync(usuario)) return Forbid();

            if (!EsSuperusuario() &&
                !string.Equals(rol, "Counter", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(rol, "Pasajero", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (string.IsNullOrWhiteSpace(rol) ||
                !await _userManager.IsInRoleAsync(usuario, rol))
                return BadRequest("El usuario no tiene ese rol.");

            if (string.Equals(rol, "Superusuario", StringComparison.OrdinalIgnoreCase))
            {
                var actuales = await _userManager.GetUsersInRoleAsync("Superusuario");
                if (actuales.Count <= 1 || usuario.Id == _userManager.GetUserId(User))
                {
                    TempData["Error"] = "No se puede eliminar el último superusuario ni quitarse el rol propio.";
                    return RedirectToAction(nameof(Asignar), new { id });
                }
            }

            var resultado = await _userManager.RemoveFromRoleAsync(usuario, rol);
            if (!resultado.Succeeded)
                TempData["Error"] = string.Join("; ", resultado.Errors.Select(e => e.Description));
            else
                TempData["Mensaje"] = "Rol retirado correctamente.";

            return RedirectToAction(nameof(Asignar), new { id });
        }
    }
}
