using MongoDB.Bson;

namespace MoldeMVC_Core.Models
{
    // Aeropuerto embebido en rutas y vuelos.
    public partial class AeropuertoEmbebido
    {
        public ObjectId AeropuertoId { get; set; }

        public string CodigoIATA { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Localidad { get; set; } = string.Empty;
    }

    public partial class FrecuenciaRuta
    {
        public int IdSql { get; set; }

        public int DiaSemana { get; set; }

        public string HoraSalida { get; set; } = string.Empty;

        public string HoraLlegada { get; set; } = string.Empty;

        public bool Activa { get; set; }
    }

    public partial class ModeloAeronave
    {
        public int IdSql { get; set; }

        public string Fabricante { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public int CapacidadAsientos { get; set; }
    }

    public partial class AsientoAeronave
    {
        public int NumeroAsiento { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int Fila { get; set; }

        public string Lado { get; set; } = string.Empty;

        public string Ubicacion { get; set; } = string.Empty;
    }

    public partial class RutaVuelo
    {
        public ObjectId RutaId { get; set; }

        public double TarifaReferencial { get; set; }

        public AeropuertoEmbebido Origen { get; set; } = new();

        public AeropuertoEmbebido Destino { get; set; } = new();
    }

    public partial class FrecuenciaVuelo
    {
        public int IdSql { get; set; }

        public int DiaSemana { get; set; }

        public string HoraSalida { get; set; } = string.Empty;

        public string HoraLlegada { get; set; } = string.Empty;
    }

    public partial class AeronaveVuelo
    {
        public ObjectId AeronaveId { get; set; }

        public string Matricula { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Fabricante { get; set; } = string.Empty;

        public int CapacidadAsientos { get; set; }
    }

    public partial class TipoDocumentoPasajero
    {
        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;
    }

    public partial class TramoItinerario
    {
        public int IdSql { get; set; }

        public int Orden { get; set; }

        public ObjectId VueloId { get; set; }
    }

    public partial class CheckIn
    {
        public int IdSql { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Estado { get; set; } = string.Empty;
    }

    public partial class PagoCompra
    {
        public int IdSql { get; set; }

        public DateTime FechaPago { get; set; }

        public double Importe { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string MetodoPago { get; set; } = string.Empty;

        public string MarcaTarjeta { get; set; } = string.Empty;

        public string UltimosCuatroDigitos { get; set; } = string.Empty;

        public string? NombreTitular { get; set; }

        public string CodigoOperacion { get; set; } = string.Empty;

        public bool EsSimulado { get; set; }

        public string? MensajeResultado { get; set; }
    }

    public partial class ComprobanteCompra
    {
        public int IdSql { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;

        public DateTime FechaEmision { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
