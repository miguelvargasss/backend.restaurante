using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Feedback.Suggestions
{
    public class UpdateSuggestionDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los detalles son obligatorios")]
        [StringLength(500, ErrorMessage = "Los detalles no pueden exceder 500 caracteres")]
        public string Details { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100)]
        public string? ContactEmail { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string Status { get; set; } = "Pendiente";

        public bool IsActive { get; set; }
    }
}
