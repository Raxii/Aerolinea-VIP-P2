using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(CompraMetadata))]
    public partial class Compra
    {
        // Reglas de negocio adicionales de esta entidad pueden ir aquí.
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class CompraMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Reserva")]
        public ObjectId ReservaId { get; set; }

        [Display(Name = "Fecha de compra")]
        public DateTime FechaCompra { get; set; }

        [Display(Name = "Importe")]
        public double Importe { get; set; }

        [Display(Name = "Moneda")]
        public string Moneda { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Clave de idempotencia")]
        public string ClaveIdempotencia { get; set; } = string.Empty;

        [Display(Name = "Pagos")]
        public List<PagoCompra> Pagos { get; set; } = new();

        [Display(Name = "Comprobantes")]
        public List<ComprobanteCompra> Comprobantes { get; set; } = new();

    }
}
