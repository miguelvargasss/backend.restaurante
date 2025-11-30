using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class SalesNote : BaseEntity
    {
        [Required(ErrorMessage = "El número de nota es requerido")]
        [StringLength(50, ErrorMessage = "El número de nota no puede exceder 50 caracteres")]
        public string NoteNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es requerida")]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal SubTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El descuento debe ser mayor o igual a 0")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
        public string? CustomerName { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observations { get; set; }

        public bool IsPaid { get; set; } = false;

        // Relaciones
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
