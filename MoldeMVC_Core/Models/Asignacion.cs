using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Asignacion
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("reservaId")]
        public ObjectId ReservaId { get; set; }

        [BsonElement("pasajeroId")]
        public ObjectId PasajeroId { get; set; }

        [BsonElement("vueloId")]
        public ObjectId VueloId { get; set; }

        [BsonElement("numeroAsiento")]
        public int NumeroAsiento { get; set; }

        [BsonElement("importeTramo")]
        public double ImporteTramo { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("fechaAsignacion")]
        public DateTime FechaAsignacion { get; set; }

        [BsonElement("checkIn")]
        public CheckIn? CheckIn { get; set; }
    }
}
