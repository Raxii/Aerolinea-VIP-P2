using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Catalogo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonIgnoreIfNull]
        [BsonElement("ocupaInventario")]
        public bool? OcupaInventario { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("estadoVisual")]
        public string? EstadoVisual { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("activo")]
        public bool? Activo { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("activa")]
        public bool? Activa { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("longitudNumero")]
        public double? LongitudNumero { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("longitudCodigoSeguridad")]
        public double? LongitudCodigoSeguridad { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("prefijosReferencia")]
        public string? PrefijosReferencia { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("rutaLogo")]
        public string? RutaLogo { get; set; }
    }
}
