using System.ComponentModel.DataAnnotations;

namespace Backend.Restaurant.DTOs.SmallBox
{
    public class CloseSmallBoxDto
    {
        [Required(ErrorMessage = "El monto final es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal FinalAmount { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? AdditionalNote { get; set; }
    }
}
