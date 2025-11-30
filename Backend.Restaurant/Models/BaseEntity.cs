using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Restaurant.Models
{

    // Sirve solo para que otras hereden de ella
    public abstract class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public DateTime? UpdatedAt { get; set; } 

        public bool IsActive { get; set; } = true; 
    }
}
