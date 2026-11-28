
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoldeMVC_Core.Utilitarios.Authorization;
using MoldeMVC_Core.Utilitarios.Services;

namespace MoldeMVC_Core.Controllers
{
    // Acceso para Administrador y Superusuario.
    [Authorize(Roles =
        PoliticasAutorizacion.RolSuperusuario + "," +
        PoliticasAutorizacion.RolAdministrador)]
    public class RolesController : Controller
    {
        private readonly GestionRolesService _gestionRolesService;

        public RolesController(
            GestionRolesService gestionRolesService)
        {
            _gestionRolesService = gestionRolesService;
        }

        // =====================================================
        // MÉTODOS AUXILIARES
        // =====================================================

        private bool EsSuperusuario()
        {
            return User.IsInRole(
                PoliticasAutorizacion.RolSuperusuario);
        }

        private string? UsuarioActualId
        {
            get
            {
                return User.FindFirstValue(
                    ClaimTypes.NameIdentifier);
            }
        }

        private async Task<IActionResult> MostrarAsignacionAsync(
            IdentityUser usuario)
        {
            ViewBag.RolesActuales =
                await _gestionRolesService.ObtenerRolesUsuarioAsync(
                    usuario.Id);

            ViewBag.RolesDisponibles =
                await _gestionRolesService.ObtenerRolesDisponiblesAsync(
                    EsSuperusuario());

            return View("Asignar", usuario);
        }

        private void AgregarErrores(
            IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }

        // =====================================================
        // 1. LISTAR ROLES
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var roles =
                await _gestionRolesService.ObtenerRolesAsync();

            return View(roles);
        }

        // =====================================================
        // 2. DETALLES DEL ROL
        // =====================================================

        public async Task<IActionResult> Details(string id)
        {
            var rol =
                await _gestionRolesService.ObtenerRolPorIdAsync(id);

            if (rol == null)
                return NotFound();

            ViewBag.Usuarios =
                await _gestionRolesService.ObtenerUsuariosEnRolAsync(
                    rol.Name ?? string.Empty);

            return View(rol);
        }

        // =====================================================
        // 3. CREAR ROL
        // SOLO SUPERUSUARIO
        // =====================================================

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new IdentityRole());
        }

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? nombre)
        {
            var resultado =
                await _gestionRolesService.CrearRolAsync(nombre);

            if (!resultado.Succeeded)
            {
                AgregarErrores(resultado);

                ViewBag.Nombre = nombre;

                return View(new IdentityRole
                {
                    Name = nombre?.Trim()
                });
            }

            TempData["Mensaje"] =
                "Rol creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // 4. EDITAR ROL
        // SOLO SUPERUSUARIO
        // =====================================================

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var rol =
                await _gestionRolesService.ObtenerRolPorIdAsync(id);

            if (rol == null)
                return NotFound();

            if (PoliticasAutorizacion.EsRolInicial(rol.Name))
                return Forbid();

            return View(rol);
        }

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            string? nombre)
        {
            var resultado =
                await _gestionRolesService.ActualizarRolAsync(
                    id,
                    nombre);

            if (resultado.Succeeded)
            {
                TempData["Mensaje"] =
                    "Rol actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            if (resultado.Errors.Any(
                e => e.Code == "RolNoEncontrado"))
                return NotFound();

            if (resultado.Errors.Any(
                e => e.Code == "RolProtegido"))
                return Forbid();

            AgregarErrores(resultado);

            var rol =
                await _gestionRolesService.ObtenerRolPorIdAsync(id);

            if (rol == null)
                return NotFound();

            rol.Name = nombre?.Trim();

            return View(rol);
        }

        // =====================================================
        // 5. ELIMINAR ROL
        // SOLO SUPERUSUARIO
        // =====================================================

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var rol =
                await _gestionRolesService.ObtenerRolPorIdAsync(id);

            if (rol == null)
                return NotFound();

            if (PoliticasAutorizacion.EsRolInicial(rol.Name))
                return Forbid();

            var usuarios =
                await _gestionRolesService.ObtenerUsuariosEnRolAsync(
                    rol.Name ?? string.Empty);

            ViewBag.CantidadUsuarios = usuarios.Count;

            return View(rol);
        }

        [Authorize(Roles = PoliticasAutorizacion.RolSuperusuario)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var resultado =
                await _gestionRolesService.EliminarRolAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join(
                    "; ",
                    resultado.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] =
                "Rol eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // 6. LISTAR USUARIOS Y SUS ROLES
        // =====================================================

        public async Task<IActionResult> Usuarios()
        {
            var usuarios =
                await _gestionRolesService.ObtenerUsuariosAsync();

            var asignaciones =
                new Dictionary<string, string>();

            var usuariosRestringidos =
                new HashSet<string>();

            foreach (var usuario in usuarios)
            {
                var roles =
                    await _gestionRolesService.ObtenerRolesUsuarioAsync(
                        usuario.Id);

                asignaciones[usuario.Id] =
                    roles.Count > 0
                        ? string.Join(", ", roles)
                        : "Sin rol";

                var puedeAdministrar =
                    await _gestionRolesService.PuedeAdministrarUsuarioAsync(
                        usuario.Id,
                        EsSuperusuario(),
                        UsuarioActualId);

                if (!puedeAdministrar)
                {
                    usuariosRestringidos.Add(usuario.Id);
                }
            }

            ViewBag.RolesUsuarios = asignaciones;

            ViewBag.UsuariosRestringidos =
                usuariosRestringidos;

            return View(usuarios);
        }

        // =====================================================
        // 7. ASIGNAR ROLES - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Asignar(string id)
        {
            var usuario =
                await _gestionRolesService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var puedeAdministrar =
                await _gestionRolesService.PuedeAdministrarUsuarioAsync(
                    id,
                    EsSuperusuario(),
                    UsuarioActualId);

            if (!puedeAdministrar)
                return Forbid();

            return await MostrarAsignacionAsync(usuario);
        }

        // =====================================================
        // 8. ASIGNAR ROLES - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(
            string id,
            string? rol)
        {
            var usuario =
                await _gestionRolesService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var resultado =
                await _gestionRolesService.AsignarRolAsync(
                    id,
                    rol,
                    UsuarioActualId,
                    EsSuperusuario());

            if (!resultado.Succeeded)
            {
                if (resultado.Errors.Any(
                    e => e.Code == "AccesoDenegado"))
                    return Forbid();

                AgregarErrores(resultado);

                return await MostrarAsignacionAsync(usuario);
            }

            TempData["Mensaje"] =
                "Rol asignado correctamente.";

            return RedirectToAction(
                nameof(Asignar),
                new { id });
        }

        // =====================================================
        // 9. RETIRAR ROLES
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Quitar(
            string id,
            string? rol)
        {
            var usuario =
                await _gestionRolesService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var resultado =
                await _gestionRolesService.QuitarRolAsync(
                    id,
                    rol,
                    UsuarioActualId,
                    EsSuperusuario());

            if (!resultado.Succeeded)
            {
                if (resultado.Errors.Any(
                    e => e.Code == "AccesoDenegado"))
                    return Forbid();

                TempData["Error"] = string.Join(
                    "; ",
                    resultado.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Mensaje"] =
                    "Rol retirado correctamente.";
            }

            return RedirectToAction(
                nameof(Asignar),
                new { id });
        }
    }
}
