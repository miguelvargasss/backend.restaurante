using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Ticket : BaseEntity
    {
        [Required(ErrorMessage = "El número de ticket es Obligatorio")]
        [StringLength(50, ErrorMessage = "El número de ticket no puede exceder 50 caracteres")]
        public string TicketNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es Obligatoria")]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El subtotal es Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal SubTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El descuento debe ser mayor o igual a 0")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "El total es Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
        public string? CustomerName { get; set; }

        [StringLength(200, ErrorMessage = "Las observaciones no pueden exceder 200 caracteres")]
        public string? Observations { get; set; }

        // Relaciones
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
