using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    public partial class Catalogo
    {
        public ObjectId Id { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool? OcupaInventario { get; set; }

        public string? EstadoVisual { get; set; }

        public bool? Activo { get; set; }

        public bool? Activa { get; set; }

        public double? LongitudNumero { get; set; }

        public double? LongitudCodigoSeguridad { get; set; }

        public string? PrefijosReferencia { get; set; }

        public string? RutaLogo { get; set; }
    }
}
