using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.Registers.Reserves
{
    public class CreateReserveDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Debe ser un número de teléfono válido")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El teléfono debe tener 9 dígitos")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de personas es obligatorio")]
        [Range(1, 100, ErrorMessage = "El número de personas debe estar entre 1 y 100")]
        public int NumberOfPeople { get; set; }

        public bool AdvancePayment { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal Amount { get; set; } = 0;

        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        public DateTime ReservationDate { get; set; }

        public int? TableId { get; set; }
        
        public int? WorkerId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
