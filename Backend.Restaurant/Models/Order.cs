using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Order : BaseEntity
    {
        [Required(ErrorMessage = "El número de orden es requerido")]
        [StringLength(50, ErrorMessage = "El número de orden no puede exceder 50 caracteres")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de la orden es requerida")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        public string Status { get; set; } = "Pendiente"; // "Pendiente", "En Proceso", "Completada", "Cancelada"

        // IDs (Limpié los espacios vacíos que tenías aquí entre líneas)
        public int? UserId { get; set; } // Cliente
        public int? TableId { get; set; } // Mesa
        public int? WorkerId { get; set; } // Mesero
        public int? PaymentMethodId { get; set; } // Método de pago

        // Datos financieros
        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor o igual a 0")]
        public decimal SubTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El descuento debe ser mayor o igual a 0")]
        public decimal Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El impuesto debe ser mayor o igual a 0")]
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        // Información adicional
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
        public string? CustomerName { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de orden no puede exceder 50 caracteres")]
        public string OrderType { get; set; } = "Local";

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observations { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        // Relaciones
        public User? User { get; set; }
        public Table? Table { get; set; }
        public Worker? Worker { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<SalesNote>? SalesNotes { get; set; }
    } 
}