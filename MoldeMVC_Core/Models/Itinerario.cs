using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Itinerario
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("referencia")]
        public string Referencia { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("tramos")]
        public List<TramoItinerario> Tramos { get; set; } = new();
    }
}
