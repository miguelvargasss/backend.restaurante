using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Claim : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del reclamo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NameClaim { get; set; } = string.Empty;

        [Required(ErrorMessage = "El detalle del reclamo es requerido")]
        [StringLength(500, ErrorMessage = "El detalle no puede exceder 500 caracteres")]
        public string DetailClaim { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del reclamo es requerida")]
        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;
    }
}
