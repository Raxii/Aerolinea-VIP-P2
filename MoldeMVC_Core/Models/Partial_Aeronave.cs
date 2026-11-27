using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoldeMVC_Core.Models
{
    [ModelMetadataType(typeof(AeronaveMetadata))]
    public partial class Aeronave
    {
        // Regla de negocio: el plano debe coincidir con la capacidad.
        public bool DistribucionAsientosValida()
        {
            if (Modelo == null || Asientos == null || Modelo.CapacidadAsientos <= 0)
                return false;

            return Asientos.Count == Modelo.CapacidadAsientos
                && Asientos.All(a => a.NumeroAsiento > 0 && !string.IsNullOrWhiteSpace(a.Codigo))
                && Asientos.Select(a => a.NumeroAsiento).Distinct().Count() == Asientos.Count
                && Asientos.Select(a => a.Codigo.Trim().ToUpperInvariant()).Distinct().Count() == Asientos.Count;
        }
    }

    // DataAnnotations para la misma entidad, sin ViewModels.
    public partial class AeronaveMetadata
    {
        [Range(1, int.MaxValue, ErrorMessage = "El ID SQL debe ser mayor que cero.")]
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Required(ErrorMessage = "Ingrese la matrícula.")]
        [Display(Name = "Matrícula")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el estado.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Modelo de aeronave")]
        public ModeloAeronave Modelo { get; set; } = new();

        [Display(Name = "Asientos")]
        public List<AsientoAeronave> Asientos { get; set; } = new();

    }
}
