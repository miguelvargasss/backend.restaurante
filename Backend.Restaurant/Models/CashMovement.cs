using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.Models
{
    // Aqui se establecen ingresan los valores de ingresos y egresos.
    public class CashMovement : BaseEntity
    {
        [Required(ErrorMessage = "El tipo de movimiento es requerido")]
        [StringLength(20, ErrorMessage = "El tipo no puede exceder 20 caracteres")]
        public string MovementType { get; set; } = string.Empty; // "Ingreso" o "Egreso"

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "El concepto es requerido")]
        [StringLength(500, ErrorMessage = "El concepto no puede exceder 500 caracteres")]
        public string Concept { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del movimiento es requerida")]
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;

        // Relaciones
        public int? SmallBoxId { get; set; }
        public SmallBox? SmallBox { get; set; }
    }
}
