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

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20)]
        public string Status { get; set; } = "Pendiente";

        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Debe ser un número de teléfono válido")]
        [StringLength(9)]
        public string? ContactPhone { get; set; }
    }
}
