using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Compra
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public ObjectId ReservaId { get; set; }

        public DateTime FechaCompra { get; set; }

        public double Importe { get; set; }

        public string Moneda { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string ClaveIdempotencia { get; set; } = string.Empty;

        public List<PagoCompra> Pagos { get; set; } = new();

        public List<ComprobanteCompra> Comprobantes { get; set; } = new();
    }
}
