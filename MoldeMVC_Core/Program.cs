
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;

using MoldeMVC_Core.Data;
using MoldeMVC_Core.Roles;
using MoldeMVC_Core.Utilitarios.Authorization;
using MoldeMVC_Core.Utilitarios.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. CONVENCIONES DE MONGODB
// =====================================================

MongoConvenciones.Registrar();

// =====================================================
// 2. CONEXIÓN SQL SERVER
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =====================================================
// 3. CONFIGURACIÓN ASP.NET CORE IDENTITY
// =====================================================

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;

    // Confirmación de cuenta
    options.SignIn.RequireConfirmedAccount = false;

    // Correo electrónico
    options.User.RequireUniqueEmail = false;

    // Nombre de usuario
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áéíóúÁÉÍÓÚñÑüÜ";

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// =====================================================
// 4. AUTORIZACIÓN Y POLÍTICAS
// =====================================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloUsuariosAdmins", policy =>
        policy.RequireRole(
            PoliticasAutorizacion.RolSuperusuario,
            PoliticasAutorizacion.RolAdministrador));
});

// =====================================================
// 5. CONTROLADORES, VISTAS Y RAZOR PAGES
// =====================================================

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

// =====================================================
// 6. REGISTRO DE SERVICIOS
// =====================================================

// Registrar una sola vez el servicio de gestión de roles
builder.Services.AddScoped<GestionRolesService>();

// =====================================================
// 7. AUTENTICACIÓN FEDERADA
// MICROSOFT Y GOOGLE
// =====================================================

var autenticacion = builder.Services.AddAuthentication();

// Credenciales Microsoft
var microsoftClientId =
    builder.Configuration["MicrosoftClientId"];

var microsoftSecret =
    builder.Configuration["MicrosoftSecretId"];

if (!string.IsNullOrWhiteSpace(microsoftClientId) &&
    !string.IsNullOrWhiteSpace(microsoftSecret))
{
    autenticacion.AddMicrosoftAccount(opciones =>
    {
        opciones.ClientId = microsoftClientId;
        opciones.ClientSecret = microsoftSecret;
    });
}

// Credenciales Google
var googleClientId =
    builder.Configuration["GoogleClientId"];

var googleSecret =
    builder.Configuration["GoogleClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleSecret))
{
    autenticacion.AddGoogle(opciones =>
    {
        opciones.ClientId = googleClientId;
        opciones.ClientSecret = googleSecret;
    });
}

// =====================================================
// 8. CONFIGURACIÓN DE COOKIES
// =====================================================

builder.Services.PostConfigure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/Identity/Account/Login";

        opciones.AccessDeniedPath =
            "/Identity/Account/AccessDenied";
    });

// =====================================================
// 9. CONTEXTO MONGODB
// =====================================================

builder.Services.AddSingleton<MongoDbContext>();

// =====================================================
// 10. CONSTRUIR APLICACIÓN
// =====================================================

var app = builder.Build();

// =====================================================
// 11. MIGRACIONES Y CONFIGURACIÓN INICIAL
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context =
            services.GetRequiredService<ApplicationDbContext>();

        // Aplicar migraciones pendientes
        await context.Database.MigrateAsync();

        // Crear roles iniciales
        await RolesUsuarios.CrearRolesAsync(services);

        // Crear usuarios iniciales
        await RolesUsuarios.CrearUsuariosAsync(
            services,
            builder.Configuration);
    }
    catch (Exception ex)
    {
        var logger =
            services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "Error al inicializar la base de datos y los roles.");
    }
}

// =====================================================
// 12. CONFIGURACIÓN DEL PIPELINE HTTP
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// =====================================================
// 13. AUTENTICACIÓN Y AUTORIZACIÓN
// =====================================================

app.UseAuthentication();

app.UseAuthorization();

// =====================================================
// 14. ARCHIVOS ESTÁTICOS
// =====================================================

app.MapStaticAssets();

// =====================================================
// 15. RUTAS MVC
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// =====================================================
// 16. RAZOR PAGES - IDENTITY
// =====================================================

app.MapRazorPages()
    .WithStaticAssets();

// =====================================================
// 17. EJECUTAR APLICACIÓN
// =====================================================

app.Run();
