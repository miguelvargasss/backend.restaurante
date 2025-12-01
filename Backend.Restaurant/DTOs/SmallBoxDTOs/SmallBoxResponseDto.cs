namespace Backend.Restaurant.DTOs.SmallBox
{
    public class SmallBoxResponseDto
    {
        public int Id { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string? AdditionalNote { get; set; }
        public bool IsClosed { get; set; }
        
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal CurrentBalance { get; set; }
        
        public List<CashMovementDto> CashMovements { get; set; } = new List<CashMovementDto>();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
