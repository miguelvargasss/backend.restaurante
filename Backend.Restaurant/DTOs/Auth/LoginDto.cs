using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public required string Email { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public required string Password { get; set; }
    }
}
