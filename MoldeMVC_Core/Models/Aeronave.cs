using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Aeronave
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("matricula")]
        public string Matricula { get; set; } = string.Empty;

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("modelo")]
        public ModeloAeronave Modelo { get; set; } = new();

        [BsonElement("asientos")]
        public List<AsientoAeronave> Asientos { get; set; } = new();
    }
}
