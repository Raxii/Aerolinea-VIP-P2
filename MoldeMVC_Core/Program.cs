using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;
using MoldeMVC_Core.Roles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//******************** Configurar la autenticación *********************************
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Desactivar los requisitos de la contraseña
    options.Password.RequireDigit = false; // No requiere dígitos
    options.Password.RequireLowercase = false; // No requiere minúsculas
    options.Password.RequireUppercase = false; // No requiere mayúsculas
    options.Password.RequireNonAlphanumeric = false; // No requiere caracteres especiales
    options.Password.RequiredLength = 3; // Longitud mínima de 3 caracteres

    // No requerimos la confirmación de cuenta para iniciar sesión
    options.SignIn.RequireConfirmedAccount = false;

    // Configuramos la validación de correo
    options.User.RequireUniqueEmail = false;

    // Permitimos espacios, tildes y ñ en el nombre de usuario (para nombres reales)
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áéíóúÁÉÍÓÚñÑüÜ";

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
//*******************************************************************************

builder.Services.AddControllersWithViews();

//************** Login Federado Con Microsoft y Facebook **********************
builder.Services.AddAuthentication()

    // Autenticación con Microsoft
    .AddMicrosoftAccount(opciones =>
    {
        opciones.ClientId = builder.Configuration["MicrosoftClientId"]!;
        opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"]!;
    })

    // Autenticación con Facebook
    //.AddFacebook(opciones =>
    //{
    //    opciones.AppId = builder.Configuration["FacebookAppId"]!;
    //    opciones.AppSecret = builder.Configuration["FacebookAppSecret"]!;
    //})

    // Autenticación con Google
    .AddGoogle(opciones =>
    {
        opciones.ClientId = builder.Configuration["GoogleClientId"]!;
        opciones.ClientSecret = builder.Configuration["GoogleClientSecret"]!;
    });

builder.Services.PostConfigure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/Identity/Account/Login";
        opciones.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

//*******************************************************************************

builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

//************** Aplicar Migraciones Automáticamente **********************
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate(); // Aplica las migraciones pendientes

        //************** Crear Roles Automáticamente **********************
        await RolesUsuarios.CrearRolesAsync(services);
        await RolesUsuarios.CrearUsuariosAsync(
            services, builder.Configuration);
        //****************************************************************
    }
    catch (Exception ex)
    {
        // Registra errores en el log en caso de que falle
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
//***********************************************************************

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days, you may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
