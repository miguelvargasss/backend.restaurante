using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Worker : BaseEntity
    {
        [Required(ErrorMessage = "El nombre es Obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string NameWorker { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es Obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        public string LastNameWorker { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es Obligatorio")]
        [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener 8 dígitos")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "El teléfono es Obligatorio")]
        [Phone(ErrorMessage = "Debe ser un número de teléfono válido")]
        [StringLength(9, ErrorMessage = "El teléfono no puede exceder 9 caracteres")] // 20 caracteres
        public string PhoneWorker { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido")]
        [StringLength(100)]
        public string? EmailWorker { get; set; }

        [Required(ErrorMessage = "El salario es Obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor o igual a 0")]
        public decimal SalaryWorker { get; set; }

        // Relaciones
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Reserve>? Reserves { get; set; }
    }
}
