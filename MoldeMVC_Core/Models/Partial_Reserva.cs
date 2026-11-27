using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(ReservaMetadata))]
    public partial class Reserva
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class ReservaMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Código de reserva")]
        public string? CodigoReserva { get; set; }

        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; } = string.Empty;

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de expiración")]
        public DateTime? FechaExpiracion { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

    }
}
