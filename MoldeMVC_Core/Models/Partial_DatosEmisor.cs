using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(DatosEmisorMetadata))]
    public partial class DatosEmisor
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class DatosEmisorMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Nombre comercial")]
        public string NombreComercial { get; set; } = string.Empty;

        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Leyenda del comprobante")]
        public string LeyendaComprobante { get; set; } = string.Empty;

    }
}
