using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Compra
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("reservaId")]
        public ObjectId ReservaId { get; set; }

        [BsonElement("fechaCompra")]
        public DateTime FechaCompra { get; set; }

        [BsonElement("importe")]
        public double Importe { get; set; }

        [BsonElement("moneda")]
        public string Moneda { get; set; } = string.Empty;

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("claveIdempotencia")]
        public string ClaveIdempotencia { get; set; } = string.Empty;

        [BsonElement("pagos")]
        public List<PagoCompra> Pagos { get; set; } = new();

        [BsonElement("comprobantes")]
        public List<ComprobanteCompra> Comprobantes { get; set; } = new();
    }
}
