using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Invoice : BaseEntity
    {
        [Required(ErrorMessage = "El número de factura es requerido")]
        [StringLength(50, ErrorMessage = "El número de factura no puede exceder 50 caracteres")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es requerida")]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal SubTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El IGV debe ser mayor o igual a 0")]
        public decimal IGV { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El cliente es requerido")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? CustomerDNI { get; set; }

        [StringLength(20)]
        public string? CustomerRUC { get; set; }

        // Relaciones
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
