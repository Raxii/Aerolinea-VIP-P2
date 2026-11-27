using System.ComponentModel.DataAnnotations;

namespace MoldeMVC_Core.Models
{
    public partial class AeropuertoMetadata
    {
        [Display(Name = "Código de registro")]
        public int IdSql { get; set; }

        [Required(ErrorMessage = "Ingrese el código IATA.")]
        [StringLength(3, MinimumLength = 3,
            ErrorMessage = "El código IATA debe tener 3 letras.")]
        [RegularExpression(@"^[A-Za-z]{3}$",
            ErrorMessage = "El código IATA solo acepta letras.")]
        [Display(Name = "Código IATA")]
        public string CodigoIATA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el nombre del aeropuerto.")]
        [Display(Name = "Nombre del aeropuerto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la localidad.")]
        [Display(Name = "Localidad")]
        public string Localidad { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
    }
}
