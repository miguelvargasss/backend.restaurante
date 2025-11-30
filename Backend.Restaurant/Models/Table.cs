using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Table : BaseEntity
    {
        [Required(ErrorMessage = "El nombre de la mesa es requerido")]
        [StringLength(20, ErrorMessage = "El nombre no puede exceder 20 caracteres")]
        public string NameTable { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ambiente es requerido")]
        [StringLength(50, ErrorMessage = "El ambiente no puede exceder 50 caracteres")]
        public string Environment { get; set; } = string.Empty;

        [Required(ErrorMessage = "La capacidad es requerida")]
        [Range(1, 50, ErrorMessage = "La capacidad debe estar entre 1 y 50 personas")]
        public int Capacity { get; set; }

        // Relaciones
        public int? LoungeId { get; set; }
        public Lounge? Lounge { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Reserve>? Reserves { get; set; }
    }
}
