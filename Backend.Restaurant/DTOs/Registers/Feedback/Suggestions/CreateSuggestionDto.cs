using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Feedback.Suggestions
{
    public class CreateSuggestionDto
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

        public DateTime SuggestionDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
