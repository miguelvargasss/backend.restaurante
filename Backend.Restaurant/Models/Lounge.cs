using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Lounge : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del ambiente es Obligatorio")]
        [StringLength(50)]
        public string NameLounge { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Table>? Tables { get; set; }
    }
}
