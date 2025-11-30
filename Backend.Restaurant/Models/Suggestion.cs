using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Suggestion : BaseEntity
    {
        [Required(ErrorMessage = "El nombre de la sugerencia es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NameSuggestion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los detalles de la sugerencia son requeridos")]
        [StringLength(500, ErrorMessage = "Los detalles no pueden exceder 500 caracteres")]
        public string DetailsSuggestion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de la sugerencia es requerida")]
        public DateTime SuggestionDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Status { get; set; } = "Pendiente";

        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100)]
        public string? ContactEmail { get; set; }
    }
}
