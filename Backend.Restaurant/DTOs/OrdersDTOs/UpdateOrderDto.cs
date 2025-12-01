using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Orders
{
    public class UpdateOrderDto
    {
        [Required(ErrorMessage = "La mesa es requerida")]
        public int TableId { get; set; }

        public int? WorkerId { get; set; }

        [Required(ErrorMessage = "El método de pago es requerido")]
        public int PaymentMethodId { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        public string Status { get; set; } = "Pendiente";

        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder 100 caracteres")]
        public string? CustomerName { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de orden no puede exceder 50 caracteres")]
        public string OrderType { get; set; } = "Local";

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observations { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El descuento debe ser mayor o igual a 0")]
        public decimal Discount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "El impuesto debe ser mayor o igual a 0")]
        public decimal Tax { get; set; } = 0;

        public bool IsPaid { get; set; } = false;
    }
}
