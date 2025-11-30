using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class Reserve : BaseEntity
    {
        [Required(ErrorMessage = "El nombre del usuario es Obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NameUserReserve { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es Obligatorio")]
        [Phone(ErrorMessage = "Debe ser un número de teléfono válido")]
        [StringLength(9)]
        public string PhoneReserve { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de personas es Obligatorio")]
        [Range(1, 100, ErrorMessage = "El número de personas debe estar entre 1 y 100")]
        public int NumberPeople { get; set; }

        public bool? AdvancePayment { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "La fecha de reserva es Obligatoria")]
        public DateTime ReservationDate { get; set; }

        // Relaciones
        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? TableId { get; set; }
        public Table? Table { get; set; }

        public int? WorkerId { get; set; }
        public Worker? Worker { get; set; }
    }
}
