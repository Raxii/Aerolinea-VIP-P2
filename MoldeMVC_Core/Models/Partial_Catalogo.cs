using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(CatalogoMetadata))]
    public partial class Catalogo
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class CatalogoMetadata
    {
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Ocupa inventario")]
        public bool? OcupaInventario { get; set; }

        [Display(Name = "Estado visual")]
        public string? EstadoVisual { get; set; }

        [Display(Name = "Activo")]
        public bool? Activo { get; set; }

        [Display(Name = "Activa")]
        public bool? Activa { get; set; }

        [Display(Name = "Longitud del número")]
        public double? LongitudNumero { get; set; }

        [Display(Name = "Longitud del código de seguridad")]
        public double? LongitudCodigoSeguridad { get; set; }

        [Display(Name = "Prefijos de referencia")]
        public string? PrefijosReferencia { get; set; }

        [Display(Name = "Ruta del logo")]
        public string? RutaLogo { get; set; }

    }
}
