using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class DatosEmisor
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("nombreComercial")]
        public string NombreComercial { get; set; } = string.Empty;

        [BsonElement("identificacion")]
        public string Identificacion { get; set; } = string.Empty;

        [BsonElement("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [BsonElement("telefono")]
        public string? Telefono { get; set; }

        [BsonElement("correo")]
        public string? Correo { get; set; }

        [BsonElement("leyendaComprobante")]
        public string LeyendaComprobante { get; set; } = string.Empty;
    }
}
