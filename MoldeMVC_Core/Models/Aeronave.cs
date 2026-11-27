using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Aeronave
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public string Matricula { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public ModeloAeronave Modelo { get; set; } = new();

        public List<AsientoAeronave> Asientos { get; set; } = new();
    }
}
