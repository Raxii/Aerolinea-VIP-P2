using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Vuelo
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public DateTime FechaOperacion { get; set; }

        public string Estado { get; set; } = string.Empty;

        public RutaVuelo Ruta { get; set; } = new();

        public FrecuenciaVuelo Frecuencia { get; set; } = new();

        public AeronaveVuelo? Aeronave { get; set; }
    }
}
