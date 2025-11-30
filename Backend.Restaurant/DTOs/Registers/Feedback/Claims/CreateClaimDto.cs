using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Feedback.Claims
{
    public class CreateClaimDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El detalle es obligatorio")]
        [StringLength(500, ErrorMessage = "El detalle no puede exceder 500 caracteres")]
        public string Detail { get; set; } = string.Empty;

        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
