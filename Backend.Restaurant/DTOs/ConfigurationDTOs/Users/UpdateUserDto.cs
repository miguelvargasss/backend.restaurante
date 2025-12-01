using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Configuration.Users
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El perfil es obligatorio")]
        [JsonPropertyName("profileId")]
        public int ProfilId { get; set; }

        public bool IsActive { get; set; }

        // Opcional: si se envía, se actualiza la contraseña
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string? Password { get; set; }
    }
}
