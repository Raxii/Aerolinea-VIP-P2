using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Itinerario
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public List<TramoItinerario> Tramos { get; set; } = new();
    }
}
