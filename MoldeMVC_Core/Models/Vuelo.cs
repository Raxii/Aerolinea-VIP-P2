using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Vuelo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("fechaOperacion")]
        public DateTime FechaOperacion { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("ruta")]
        public RutaVuelo Ruta { get; set; } = new();

        [BsonElement("frecuencia")]
        public FrecuenciaVuelo Frecuencia { get; set; } = new();

        [BsonElement("aeronave")]
        public AeronaveVuelo? Aeronave { get; set; }
    }
}
