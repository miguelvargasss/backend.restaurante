using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class OrderDetail : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, ErrorMessage = "El nombre del producto no puede exceder 100 caracteres")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal SubTotal { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observations { get; set; }

        [StringLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres")]
        public string Status { get; set; } = "Pendiente"; // "Pendiente", "En Preparación", "Listo", "Entregado"

        // Relaciones
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
