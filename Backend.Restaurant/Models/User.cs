using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class User : BaseEntity
    {
        [Required(ErrorMessage = "El nombre es Obligatorio")]
        [StringLength(50)]
        public string NameUser { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es Obligatorio")]
        [StringLength(50)]
        public string LastNameUser { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es Obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es Obligatoria")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime? LastLogin { get; set; }

        // Relaciones
        public int? ProfilId { get; set; }
        public Profil? Profil { get; set; }

        public ICollection<Reserve>? Reserves { get; set; }
        public ICollection<SmallBox>? SmallBoxes { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
