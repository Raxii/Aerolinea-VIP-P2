using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Reserva
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("codigoReserva")]
        public string? CodigoReserva { get; set; }

        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("fechaExpiracion")]
        public DateTime? FechaExpiracion { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
