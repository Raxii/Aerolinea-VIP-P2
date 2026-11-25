using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Ruta
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("origen")]
        public AeropuertoEmbebido Origen { get; set; } = new();

        [BsonElement("destino")]
        public AeropuertoEmbebido Destino { get; set; } = new();

        [BsonElement("tarifaReferencial")]
        public double TarifaReferencial { get; set; }

        [BsonElement("activa")]
        public bool Activa { get; set; }

        [BsonElement("frecuencias")]
        public List<FrecuenciaRuta> Frecuencias { get; set; } = new();
    }
}
