using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(PasajeroMetadata))]
    public partial class Pasajero
    {
        // Regla de presentación: se calcula sin almacenarse en MongoDB.
        public string ObtenerNombreCompleto()
        {
            return $"{Nombres} {Apellidos}".Trim();
        }
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class PasajeroMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Tipo de documento")]
        public TipoDocumentoPasajero TipoDocumento { get; set; } = new();

        [Display(Name = "Número de documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

    }
}
