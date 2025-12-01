using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Cart.Categories
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
