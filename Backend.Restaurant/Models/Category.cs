using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string NameCategory { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Description { get; set; }

        // Relaciones
        public ICollection<Product>? Products { get; set; }
    }
}
