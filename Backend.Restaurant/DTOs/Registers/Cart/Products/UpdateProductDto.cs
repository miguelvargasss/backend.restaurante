using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Cart.Products
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
