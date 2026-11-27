using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Pasajero
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public TipoDocumentoPasajero TipoDocumento { get; set; } = new();

        public string NumeroDocumento { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
