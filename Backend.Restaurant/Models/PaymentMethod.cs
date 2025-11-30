using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class PaymentMethod : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del método de pago es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string NamePaymentMethod { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Description { get; set; }

        public bool RequiresAuthorization { get; set; } = false;

        // Relaciones
        public ICollection<Order>? Orders { get; set; }
    }
}

