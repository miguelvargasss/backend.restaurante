using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Auth
{
    public class InitializeDto
    {
        [Required(ErrorMessage = "El nombre del perfil es obligatorio")]
        [StringLength(50)]
        public string ProfileName { get; set; } = "Administrador";
        
        [Required(ErrorMessage = "La descripción del perfil es obligatoria")]
        [StringLength(200)]
        public string ProfileDescription { get; set; } = "Perfil con acceso administrativo completo";
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50)]
        public string UserLastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}