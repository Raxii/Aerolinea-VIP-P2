using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Asignacion
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public ObjectId ReservaId { get; set; }

        public ObjectId PasajeroId { get; set; }

        public ObjectId VueloId { get; set; }

        public int NumeroAsiento { get; set; }

        public double ImporteTramo { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaAsignacion { get; set; }

        public CheckIn? CheckIn { get; set; }
    }
}
