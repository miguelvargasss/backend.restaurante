namespace Backend.Restaurant.DTOs.SmallBox
{
    public class CashMovementDto
    {
        public int Id { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Concept { get; set; } = string.Empty;
        public DateTime MovementDate { get; set; }
    }
}
