using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(AeropuertoMetadata))]
    public partial class Aeropuerto
    {
        // Regla de negocio: IATA estándar de tres letras.
        public bool TieneCodigoIataValido()
        {
            return !string.IsNullOrWhiteSpace(CodigoIATA)
                && System.Text.RegularExpressions.Regex.IsMatch(CodigoIATA.Trim(), @"^[A-Za-z]{3}$");
        }
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class AeropuertoMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Required(ErrorMessage = "Ingrese el código IATA.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código IATA debe tener exactamente 3 letras.")]
        [RegularExpression(@"^[A-Za-z]{3}$", ErrorMessage = "El código IATA debe contener solo letras.")]
        [Display(Name = "Código IATA")]
        public string CodigoIATA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el nombre del aeropuerto.")]
        [Display(Name = "Nombre del aeropuerto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la localidad.")]
        [Display(Name = "Localidad")]
        public string Localidad { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

    }
}
