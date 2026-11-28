
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Utilitarios.Authorization;

namespace MoldeMVC_Core.Utilitarios.Services
{
    public class GestionRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public GestionRolesService(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // =====================================================
        // 1. CONSULTAS DE ROLES
        // =====================================================

        public async Task<List<IdentityRole>> ObtenerRolesAsync()
        {
            return await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IdentityRole?> ObtenerRolPorIdAsync(
            string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<List<IdentityRole>>
            ObtenerRolesDisponiblesAsync(bool esSuperusuario)
        {
            var roles = await ObtenerRolesAsync();

            if (esSuperusuario)
                return roles;

            return roles
                .Where(r =>
                    PoliticasAutorizacion
                        .EsRolPermitidoParaAdministrador(r.Name))
                .ToList();
        }

        // =====================================================
        // 2. CONSULTAS DE USUARIOS
        // =====================================================

        public async Task<List<IdentityUser>> ObtenerUsuariosAsync()
        {
            return await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<IdentityUser?> ObtenerUsuarioPorIdAsync(
            string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IList<IdentityUser>>
            ObtenerUsuariosEnRolAsync(string nombreRol)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
                return new List<IdentityUser>();

            return await _userManager.GetUsersInRoleAsync(
                nombreRol);
        }

        public async Task<IList<string>>
            ObtenerRolesUsuarioAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return new List<string>();

            return await _userManager.GetRolesAsync(usuario);
        }

        // =====================================================
        // 3. REGLAS DE NEGOCIO Y AUTORIZACIÓN
        // =====================================================

        public async Task<bool> PuedeAdministrarUsuarioAsync(
            string id,
            bool esSuperusuario,
            string? usuarioActualId)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return false;

            // El superusuario puede gestionar cualquier cuenta.
            if (esSuperusuario)
                return true;

            // El administrador no puede modificar su propia cuenta.
            if (!string.IsNullOrWhiteSpace(usuarioActualId) &&
                string.Equals(
                    usuario.Id,
                    usuarioActualId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            // El administrador no puede gestionar cuentas
            // que tengan rol Superusuario o Administrador.
            return !roles.Any(
                PoliticasAutorizacion.EsRolPrivilegiado);
        }

        // =====================================================
        // 4. CREAR ROL
        // =====================================================

        public async Task<IdentityResult> CrearRolAsync(
            string? nombre)
        {
            if (!NombreValido(nombre))
            {
                return Error(
                    "NombreInvalido",
                    "Ingrese un nombre de rol de hasta 50 caracteres.");
            }

            nombre = nombre!.Trim();

            if (PoliticasAutorizacion.EsRolInicial(nombre))
            {
                return Error(
                    "RolReservado",
                    "El nombre corresponde a un rol inicial del sistema.");
            }

            if (await _roleManager.RoleExistsAsync(nombre))
            {
                return Error(
                    "RolDuplicado",
                    "Ya existe un rol con ese nombre.");
            }

            var rol = new IdentityRole(nombre);

            return await _roleManager.CreateAsync(rol);
        }

        // =====================================================
        // 5. EDITAR ROL
        // =====================================================

        public async Task<IdentityResult> ActualizarRolAsync(
            string id,
            string? nombre)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                return Error(
                    "RolNoEncontrado",
                    "El rol solicitado no existe.");
            }

            if (PoliticasAutorizacion.EsRolInicial(rol.Name))
            {
                return Error(
                    "RolProtegido",
                    "No se permite modificar los cuatro roles iniciales.");
            }

            if (!NombreValido(nombre))
            {
                return Error(
                    "NombreInvalido",
                    "Ingrese un nombre de rol de hasta 50 caracteres.");
            }

            nombre = nombre!.Trim();

            if (PoliticasAutorizacion.EsRolInicial(nombre))
            {
                return Error(
                    "RolReservado",
                    "No puede utilizar el nombre de un rol inicial.");
            }

            if (!string.Equals(
                    rol.Name,
                    nombre,
                    StringComparison.OrdinalIgnoreCase)
                && await _roleManager.RoleExistsAsync(nombre))
            {
                return Error(
                    "RolDuplicado",
                    "Ya existe un rol con ese nombre.");
            }

            rol.Name = nombre;

            return await _roleManager.UpdateAsync(rol);
        }

        // =====================================================
        // 6. ELIMINAR ROL
        // =====================================================

        public async Task<IdentityResult> EliminarRolAsync(
            string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                return Error(
                    "RolNoEncontrado",
                    "El rol solicitado no existe.");
            }

            if (PoliticasAutorizacion.EsRolInicial(rol.Name))
            {
                return Error(
                    "RolProtegido",
                    "No se pueden eliminar los roles iniciales.");
            }

            var usuarios = await _userManager.GetUsersInRoleAsync(
                rol.Name!);

            if (usuarios.Count > 0)
            {
                return Error(
                    "RolConUsuarios",
                    "No se puede eliminar un rol que tenga usuarios asignados.");
            }

            return await _roleManager.DeleteAsync(rol);
        }

        // =====================================================
        // 7. ASIGNAR ROL
        // =====================================================

        public async Task<IdentityResult> AsignarRolAsync(
            string usuarioId,
            string? nombreRol,
            string? usuarioActualId,
            bool esSuperusuario)
        {
            var usuario = await _userManager.FindByIdAsync(
                usuarioId);

            if (usuario == null)
            {
                return Error(
                    "UsuarioNoEncontrado",
                    "El usuario no existe.");
            }

            if (!await PuedeAdministrarUsuarioAsync(
                    usuarioId,
                    esSuperusuario,
                    usuarioActualId))
            {
                return Error(
                    "AccesoDenegado",
                    "No tiene autorización para administrar este usuario.");
            }

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                return Error(
                    "RolInvalido",
                    "Seleccione un rol válido.");
            }

            var rol = await _roleManager.FindByNameAsync(
                nombreRol.Trim());

            if (rol == null)
            {
                return Error(
                    "RolNoEncontrado",
                    "El rol seleccionado no existe.");
            }

            // El administrador solamente puede asignar
            // Counter y Pasajero.
            if (!esSuperusuario &&
                !PoliticasAutorizacion
                    .EsRolPermitidoParaAdministrador(rol.Name))
            {
                return Error(
                    "PrivilegiosInsuficientes",
                    "El administrador no puede asignar roles privilegiados.");
            }

            if (await _userManager.IsInRoleAsync(
                    usuario,
                    rol.Name!))
            {
                return IdentityResult.Success;
            }

            return await _userManager.AddToRoleAsync(
                usuario,
                rol.Name!);
        }

        // =====================================================
        // 8. RETIRAR ROL
        // =====================================================

        public async Task<IdentityResult> QuitarRolAsync(
            string usuarioId,
            string? nombreRol,
            string? usuarioActualId,
            bool esSuperusuario)
        {
            var usuario = await _userManager.FindByIdAsync(
                usuarioId);

            if (usuario == null)
            {
                return Error(
                    "UsuarioNoEncontrado",
                    "El usuario no existe.");
            }

            if (!await PuedeAdministrarUsuarioAsync(
                    usuarioId,
                    esSuperusuario,
                    usuarioActualId))
            {
                return Error(
                    "AccesoDenegado",
                    "No tiene autorización para administrar este usuario.");
            }

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                return Error(
                    "RolInvalido",
                    "Seleccione un rol válido.");
            }

            var rol = await _roleManager.FindByNameAsync(
                nombreRol.Trim());

            if (rol == null)
            {
                return Error(
                    "RolNoEncontrado",
                    "El rol seleccionado no existe.");
            }

            // El administrador únicamente puede retirar
            // los roles Counter y Pasajero.
            if (!esSuperusuario &&
                !PoliticasAutorizacion
                    .EsRolPermitidoParaAdministrador(rol.Name))
            {
                return Error(
                    "PrivilegiosInsuficientes",
                    "No puede retirar roles privilegiados.");
            }

            if (!await _userManager.IsInRoleAsync(
                    usuario,
                    rol.Name!))
            {
                return Error(
                    "RolNoAsignado",
                    "El usuario no tiene asignado ese rol.");
            }

            // Protección del rol Superusuario.
            if (string.Equals(
                    rol.Name,
                    PoliticasAutorizacion.RolSuperusuario,
                    StringComparison.OrdinalIgnoreCase))
            {
                // Nadie puede quitarse su propio rol Superusuario.
                if (string.Equals(
                        usuario.Id,
                        usuarioActualId,
                        StringComparison.Ordinal))
                {
                    return Error(
                        "Autorrevocacion",
                        "No puede retirar su propio rol de Superusuario.");
                }

                // No se puede eliminar el último superusuario.
                var superusuarios =
                    await _userManager.GetUsersInRoleAsync(
                        PoliticasAutorizacion.RolSuperusuario);

                if (superusuarios.Count <= 1)
                {
                    return Error(
                        "UltimoSuperusuario",
                        "No se puede eliminar el último superusuario.");
                }
            }

            return await _userManager.RemoveFromRoleAsync(
                usuario,
                rol.Name!);
        }

        // =====================================================
        // 9. VALIDACIONES AUXILIARES
        // =====================================================

        private static bool NombreValido(string? nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre)
                   && nombre.Trim().Length <= 50;
        }

        private static IdentityResult Error(
            string codigo,
            string descripcion)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = codigo,
                    Description = descripcion
                });
        }
    }
}
