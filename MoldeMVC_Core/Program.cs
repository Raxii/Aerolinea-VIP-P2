
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;
using MoldeMVC_Core.Roles;

var builder = WebApplication.CreateBuilder(args);

//************** REGISTRAR CONVENCIONES MONGODB **********************
// Debe ejecutarse antes de utilizar MongoDbContext.
// Permite trabajar sin atributos BSON en los modelos.

MongoConvenciones.Registrar();

//********************************************************************

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//******************** CONFIGURAR AUTENTICACION **********************
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Desactivar los requisitos de la contraseña
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;

    // No requerimos confirmación de cuenta
    options.SignIn.RequireConfirmedAccount = false;

    // Configuramos la validación de correo
    options.User.RequireUniqueEmail = false;

    // Permitimos espacios, tildes y ñ
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áéíóúÁÉÍÓÚñÑüÜ";

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

//********************************************************************

builder.Services.AddControllersWithViews();

//******************** LOGIN FEDERADO ********************************
builder.Services.AddAuthentication()

    // Autenticación con Microsoft
    .AddMicrosoftAccount(opciones =>
    {
        opciones.ClientId =
            builder.Configuration["MicrosoftClientId"]!;

        opciones.ClientSecret =
            builder.Configuration["MicrosoftSecretId"]!;
    })

    // Autenticación con Google
    .AddGoogle(opciones =>
    {
        opciones.ClientId =
            builder.Configuration["GoogleClientId"]!;

        opciones.ClientSecret =
            builder.Configuration["GoogleClientSecret"]!;
    });

//******************** CONFIGURACION DE COOKIES **********************
builder.Services.PostConfigure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/Identity/Account/Login";
        opciones.AccessDeniedPath =
            "/Identity/Account/AccessDenied";
    });

//********************************************************************

//******************** CONTEXTO MONGODB ******************************
builder.Services.AddSingleton<MongoDbContext>();

//********************************************************************

var app = builder.Build();

//******************** MIGRACIONES Y ROLES ***************************
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context =
            services.GetRequiredService<ApplicationDbContext>();

        // Aplicar migraciones pendientes de SQL Server
        context.Database.Migrate();

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
            "An error occurred while migrating the database.");
    }
}

//********************************************************************

//******************** CONFIGURACION HTTP ****************************
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

//******************** AUTENTICACION Y AUTORIZACION *******************
app.UseAuthentication();
app.UseAuthorization();

//********************************************************************

app.MapStaticAssets();

//******************** RUTAS MVC ************************************
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//******************** RAZOR PAGES IDENTITY **************************
app.MapRazorPages()
   .WithStaticAssets();

//********************************************************************

app.Run();
