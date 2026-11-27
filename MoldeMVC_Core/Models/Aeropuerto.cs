using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Aeropuerto
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public string CodigoIATA { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Localidad { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
