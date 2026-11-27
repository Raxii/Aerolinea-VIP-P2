using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(RutaMetadata))]
    public partial class Ruta
    {
        // Regla de negocio: no se permite origen igual a destino.
        public bool TieneOrigenYDestinoDistintos()
        {
            return Origen != null && Destino != null
                && Origen.AeropuertoId != MongoDB.Bson.ObjectId.Empty
                && Destino.AeropuertoId != MongoDB.Bson.ObjectId.Empty
                && Origen.AeropuertoId != Destino.AeropuertoId;
        }
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class RutaMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Display(Name = "Origen")]
        public AeropuertoEmbebido Origen { get; set; } = new();

        [Display(Name = "Destino")]
        public AeropuertoEmbebido Destino { get; set; } = new();

        [Display(Name = "Tarifa referencial")]
        public double TarifaReferencial { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; }

        [Display(Name = "Frecuencias")]
        public List<FrecuenciaRuta> Frecuencias { get; set; } = new();

    }
}
