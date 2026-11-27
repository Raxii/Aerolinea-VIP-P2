using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(ItinerarioMetadata))]
    public partial class Itinerario
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class ItinerarioMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Referencia")]
        public string Referencia { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Tramos")]
        public List<TramoItinerario> Tramos { get; set; } = new();

    }
}
