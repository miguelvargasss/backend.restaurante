using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class PageAccount : BaseEntity
    {
        [Required(ErrorMessage = "El DNI es requerido")]
        [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener 8 dígitos")]
        public int DNIpage { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NameUserPage { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [StringLength(200, ErrorMessage = "Las observaciones no pueden exceder 200 caracteres")]
        public string? Observations { get; set; }
    }
}
