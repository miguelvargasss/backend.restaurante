using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Tables
{
    public class CreateTableDto
    {
        [Required(ErrorMessage = "El nombre de la mesa es obligatorio")]
        [StringLength(20, ErrorMessage = "El nombre no puede exceder 20 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ambiente es obligatorio")]
        [StringLength(50, ErrorMessage = "El ambiente no puede exceder 50 caracteres")]
        public string Environment { get; set; } = string.Empty;

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        [Range(1, 50, ErrorMessage = "La capacidad debe estar entre 1 y 50 personas")]
        public int Capacity { get; set; }

        public int? LoungeId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
