namespace Backend.Restaurant.DTOs.Registers.Reserves
{
    public class ReserveResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public bool AdvancePayment { get; set; }
        public decimal Amount { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsActive { get; set; }
        public int? TableId { get; set; }
        public string? TableName { get; set; }
        public int? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
