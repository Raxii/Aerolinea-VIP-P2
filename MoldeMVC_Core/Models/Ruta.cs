using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Ruta
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public AeropuertoEmbebido Origen { get; set; } = new();

        public AeropuertoEmbebido Destino { get; set; } = new();

        public double TarifaReferencial { get; set; }

        public bool Activa { get; set; }

        public List<FrecuenciaRuta> Frecuencias { get; set; } = new();
    }
}
