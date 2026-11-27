
using Microsoft.AspNetCore.Identity;

namespace MoldeMVC_Core.Roles
{
    public class RolesUsuarios
    {
        //************** CREAR ROLES AUTOMATICAMENTE **************
        public static async Task CrearRolesAsync(
            IServiceProvider services)
        {
            var roleManager = services
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles =
            {
                "Superusuario",
                "Administrador",
                "Counter",
                "Pasajero"
            };

            foreach (string nombreRol in roles)
            {
                if (!await roleManager.RoleExistsAsync(nombreRol))
                {
                    var resultado = await roleManager.CreateAsync(
                        new IdentityRole(nombreRol));

                    if (!resultado.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"No se pudo crear el rol {nombreRol}");
                    }
                }
            }
        }

        //************** CREAR USUARIOS AUTOMATICAMENTE ***********
        public static async Task CrearUsuariosAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            var userManager = services
                .GetRequiredService<UserManager<IdentityUser>>();

            // Contraseña inicial solo para la práctica local
            string clave = "123";

            var usuarios = new[]
            {
                (
                    Rol: "Superusuario",
                    Usuario: "superusuario",
                    Correo: "superusuario@aerolineavip.local"
                ),
                (
                    Rol: "Administrador",
                    Usuario: "administrador",
                    Correo: "administrador@aerolineavip.local"
                ),
                (
                    Rol: "Counter",
                    Usuario: "counter",
                    Correo: "counter@aerolineavip.local"
                ),
                (
                    Rol: "Pasajero",
                    Usuario: "pasajero",
                    Correo: "pasajero@aerolineavip.local"
                )
            };

            foreach (var item in usuarios)
            {
                // Buscar por el nombre de usuario correcto
                var usuario = await userManager.FindByNameAsync(
                    item.Usuario);

                // Buscar el usuario creado anteriormente con el correo
                if (usuario == null)
                {
                    usuario = await userManager.FindByNameAsync(
                        item.Correo);

                    // Actualizar el nombre sin eliminar la cuenta
                    if (usuario != null)
                    {
                        var resultadoNombre =
                            await userManager.SetUserNameAsync(
                                usuario, item.Usuario);

                        if (!resultadoNombre.Succeeded)
                        {
                            throw new InvalidOperationException(
                                string.Join("; ",
                                    resultadoNombre.Errors.Select(
                                        e => e.Description)));
                        }
                    }
                }

                // Crear solo si todavía no existe
                if (usuario == null)
                {
                    usuario = new IdentityUser
                    {
                        UserName = item.Usuario,
                        Email = item.Correo
                    };

                    var resultado = await userManager.CreateAsync(
                        usuario, clave);

                    if (!resultado.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ",
                                resultado.Errors.Select(
                                    e => e.Description)));
                    }

                    // Asignar su rol correspondiente
                    var resultadoRol =
                        await userManager.AddToRoleAsync(
                            usuario, item.Rol);

                    if (!resultadoRol.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join("; ",
                                resultadoRol.Errors.Select(
                                    e => e.Description)));
                    }
                }
                else
                {
                    // No conceder nuevos privilegios
                    // a cuentas existentes sin revisión
                    if (!await userManager.IsInRoleAsync(
                        usuario, item.Rol))
                    {
                        throw new InvalidOperationException(
                            $"El usuario {item.Usuario} ya existe " +
                            $"sin el rol {item.Rol}.");
                    }
                }
            }
        }
    }
}
