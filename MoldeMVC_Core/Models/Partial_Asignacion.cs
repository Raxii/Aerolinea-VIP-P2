using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(AsignacionMetadata))]
    public partial class Asignacion
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class AsignacionMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Reserva")]
        public ObjectId ReservaId { get; set; }

        [Display(Name = "Pasajero")]
        public ObjectId PasajeroId { get; set; }

        [Display(Name = "Vuelo")]
        public ObjectId VueloId { get; set; }

        [Display(Name = "Número de asiento")]
        public int NumeroAsiento { get; set; }

        [Display(Name = "Importe del tramo")]
        public double ImporteTramo { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Fecha de asignación")]
        public DateTime FechaAsignacion { get; set; }

        [Display(Name = "Check-in")]
        public CheckIn? CheckIn { get; set; }

    }
}
