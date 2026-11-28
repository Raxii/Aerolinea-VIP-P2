
using System;
using System.Linq;

namespace MoldeMVC_Core.Utilitarios.Authorization
{
    public static class PoliticasAutorizacion
    {
        // Roles principales del sistema
        public const string RolSuperusuario = "Superusuario";
        public const string RolAdministrador = "Administrador";
        public const string RolCounter = "Counter";
        public const string RolPasajero = "Pasajero";

        // Los cuatro roles iniciales no se pueden modificar
        // ni eliminar.
        private static readonly string[] RolesIniciales =
        {
            RolSuperusuario,
            RolAdministrador,
            RolCounter,
            RolPasajero
        };

        // Verificar si un rol es inicial
        public static bool EsRolInicial(string? nombre)
        {
            return RolesIniciales.Any(rol =>
                string.Equals(
                    rol,
                    nombre,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Identificar roles privilegiados
        public static bool EsRolPrivilegiado(string? nombre)
        {
            return string.Equals(
                       nombre,
                       RolSuperusuario,
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       nombre,
                       RolAdministrador,
                       StringComparison.OrdinalIgnoreCase);
        }

        // Roles que puede asignar o retirar un administrador
        public static bool EsRolPermitidoParaAdministrador(
            string? nombre)
        {
            return string.Equals(
                       nombre,
                       RolCounter,
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       nombre,
                       RolPasajero,
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
