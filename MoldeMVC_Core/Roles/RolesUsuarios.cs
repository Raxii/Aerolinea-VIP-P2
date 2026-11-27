
using Microsoft.AspNetCore.Identity;

namespace MoldeMVC_Core.Roles
{
    public class RolesUsuarios
    {
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
    }
}
