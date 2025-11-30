using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NameProduct { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string DescriptionProduct { get; set; } = string.Empty;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(200)]
        public string? ImageUrl { get; set; }

        // Relaciones
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
