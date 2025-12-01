using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Lounges
{
    public class UpdateLoungeDto
    {
        [Required(ErrorMessage = "El nombre del salón es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
