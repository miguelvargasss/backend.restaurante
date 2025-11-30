using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Configuration.Profiles
{
    public class CreateProfileDto
    {
        [Required(ErrorMessage = "El nombre del perfil es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        public bool HasAdminAccess { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
    }
}
