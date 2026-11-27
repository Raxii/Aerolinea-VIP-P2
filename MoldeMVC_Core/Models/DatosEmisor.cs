using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class DatosEmisor
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public string NombreComercial { get; set; } = string.Empty;

        public string Identificacion { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public string LeyendaComprobante { get; set; } = string.Empty;
    }
}
