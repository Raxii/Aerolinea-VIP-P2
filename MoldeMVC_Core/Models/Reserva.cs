using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Reserva
    {
        public ObjectId Id { get; set; }

        public int IdSql { get; set; }

        public string? CodigoReserva { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaExpiracion { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
