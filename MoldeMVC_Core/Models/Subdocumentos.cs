using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    // Aeropuerto embebido en rutas y vuelos.
    [BsonIgnoreExtraElements]
    public class AeropuertoEmbebido
    {
        [BsonElement("aeropuertoId")]
        public ObjectId AeropuertoId { get; set; }

        [BsonElement("codigoIATA")]
        public string CodigoIATA { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("localidad")]
        public string Localidad { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class FrecuenciaRuta
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("diaSemana")]
        public int DiaSemana { get; set; }

        [BsonElement("horaSalida")]
        public string HoraSalida { get; set; } = string.Empty;

        [BsonElement("horaLlegada")]
        public string HoraLlegada { get; set; } = string.Empty;

        [BsonElement("activa")]
        public bool Activa { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ModeloAeronave
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("fabricante")]
        public string Fabricante { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("capacidadAsientos")]
        public int CapacidadAsientos { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class AsientoAeronave
    {
        [BsonElement("numeroAsiento")]
        public int NumeroAsiento { get; set; }

        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [BsonElement("fila")]
        public int Fila { get; set; }

        [BsonElement("lado")]
        public string Lado { get; set; } = string.Empty;

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class RutaVuelo
    {
        [BsonElement("rutaId")]
        public ObjectId RutaId { get; set; }

        [BsonElement("tarifaReferencial")]
        public double TarifaReferencial { get; set; }

        [BsonElement("origen")]
        public AeropuertoEmbebido Origen { get; set; } = new();

        [BsonElement("destino")]
        public AeropuertoEmbebido Destino { get; set; } = new();
    }

    [BsonIgnoreExtraElements]
    public class FrecuenciaVuelo
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("diaSemana")]
        public int DiaSemana { get; set; }

        [BsonElement("horaSalida")]
        public string HoraSalida { get; set; } = string.Empty;

        [BsonElement("horaLlegada")]
        public string HoraLlegada { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class AeronaveVuelo
    {
        [BsonElement("aeronaveId")]
        public ObjectId AeronaveId { get; set; }

        [BsonElement("matricula")]
        public string Matricula { get; set; } = string.Empty;

        [BsonElement("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [BsonElement("fabricante")]
        public string Fabricante { get; set; } = string.Empty;

        [BsonElement("capacidadAsientos")]
        public int CapacidadAsientos { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class TipoDocumentoPasajero
    {
        [BsonElement("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class TramoItinerario
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("orden")]
        public int Orden { get; set; }

        [BsonElement("vueloId")]
        public ObjectId VueloId { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CheckIn
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class PagoCompra
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("fechaPago")]
        public DateTime FechaPago { get; set; }

        [BsonElement("importe")]
        public double Importe { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;

        [BsonElement("metodoPago")]
        public string MetodoPago { get; set; } = string.Empty;

        [BsonElement("marcaTarjeta")]
        public string MarcaTarjeta { get; set; } = string.Empty;

        [BsonElement("ultimosCuatroDigitos")]
        public string UltimosCuatroDigitos { get; set; } = string.Empty;

        [BsonElement("nombreTitular")]
        public string? NombreTitular { get; set; }

        [BsonElement("codigoOperacion")]
        public string CodigoOperacion { get; set; } = string.Empty;

        [BsonElement("esSimulado")]
        public bool EsSimulado { get; set; }

        [BsonElement("mensajeResultado")]
        public string? MensajeResultado { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ComprobanteCompra
    {
        [BsonElement("idSql")]
        public int IdSql { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [BsonElement("numero")]
        public string Numero { get; set; } = string.Empty;

        [BsonElement("fechaEmision")]
        public DateTime FechaEmision { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
