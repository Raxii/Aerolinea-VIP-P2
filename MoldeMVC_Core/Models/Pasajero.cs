using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Pasajero
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("tipoDocumento")]
        public TipoDocumentoPasajero TipoDocumento { get; set; } = new();

        [BsonElement("numeroDocumento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [BsonElement("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [BsonElement("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [BsonElement("fechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [BsonElement("correo")]
        public string? Correo { get; set; }

        [BsonElement("telefono")]
        public string? Telefono { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }
    }
}
