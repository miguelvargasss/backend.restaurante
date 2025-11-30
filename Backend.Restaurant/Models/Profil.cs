using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Profil : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del perfil es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string NameProfil { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción del perfil es requerida")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string DescriptionProfil { get; set; } = string.Empty;

        public bool HasAdminAccess { get; set; } = false;

        // Relaciones
        public ICollection<User>? Users { get; set; }
    }
}
