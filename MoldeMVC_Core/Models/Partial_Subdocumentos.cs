using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(ModeloAeronaveMetadata))]
    public partial class ModeloAeronave
    {
    }

    public partial class ModeloAeronaveMetadata
    {
        [Display(Name = "Código del modelo")]
        public int IdSql { get; set; }

        [Required(ErrorMessage = "Ingrese el fabricante.")]
        [Display(Name = "Fabricante")]
        public string Fabricante { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el nombre del modelo.")]
        [Display(Name = "Modelo")]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, 7, ErrorMessage = "La capacidad debe estar entre 1 y 7 asientos.")]
        [Display(Name = "Capacidad de asientos")]
        public int CapacidadAsientos { get; set; }
    }

    [ModelMetadataType(typeof(AsientoAeronaveMetadata))]
    public partial class AsientoAeronave
    {
    }

    public partial class AsientoAeronaveMetadata
    {
        [Range(1, 7, ErrorMessage = "El número de asiento debe estar entre 1 y 7.")]
        [Display(Name = "Número de asiento")]
        public int NumeroAsiento { get; set; }

        [Required(ErrorMessage = "Ingrese el código del asiento.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Display(Name = "Fila")]
        public int Fila { get; set; }

        [Required(ErrorMessage = "Ingrese el lado del asiento.")]
        [Display(Name = "Lado")]
        public string Lado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la ubicación del asiento.")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;
    }
}
