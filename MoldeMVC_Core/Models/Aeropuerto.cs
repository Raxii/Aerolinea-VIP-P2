using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Aeropuerto
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("codigoIATA")]
        public string CodigoIATA { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("localidad")]
        public string Localidad { get; set; } = string.Empty;

        [BsonElement("activo")]
        public bool Activo { get; set; }
    }
}
