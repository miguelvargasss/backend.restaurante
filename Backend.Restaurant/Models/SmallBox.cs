using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    public class SmallBox : BaseEntity
    {
        [Required(ErrorMessage = "El monto inicial es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal InitialAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto final debe ser mayor o igual a 0")]
        public decimal FinalAmount { get; set; }

        [Required(ErrorMessage = "La fecha de apertura es requerida")]
        public DateTime OpeningDate { get; set; } = DateTime.UtcNow;

        public DateTime? ClosingDate { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? AdditionalNote { get; set; }

        public bool IsClosed { get; set; } = false;

        // Relaciones
        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<CashMovement>? CashMovements { get; set; }
    }
}
