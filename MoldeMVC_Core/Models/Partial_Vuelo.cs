using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(VueloMetadata))]
    public partial class Vuelo
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class VueloMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Fecha de operación")]
        public DateTime FechaOperacion { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Ruta")]
        public RutaVuelo Ruta { get; set; } = new();

        [Display(Name = "Frecuencia")]
        public FrecuenciaVuelo Frecuencia { get; set; } = new();

        [Display(Name = "Aeronave")]
        public AeronaveVuelo? Aeronave { get; set; }

    }
}
