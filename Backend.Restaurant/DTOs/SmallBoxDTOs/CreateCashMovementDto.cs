using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.SmallBox
{
    public class CreateCashMovementDto
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

        [Required(ErrorMessage = "La caja chica es requerida")]
        public int SmallBoxId { get; set; }
    }
}
