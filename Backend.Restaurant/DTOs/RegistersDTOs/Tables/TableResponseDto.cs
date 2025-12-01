namespace Backend.Restaurant.DTOs.Registers.Tables
{
    public class TableResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public int? LoungeId { get; set; }
        public string? LoungeName { get; set; }
        
        // Campos de ocupación de mesa
        public bool IsOccupied { get; set; }
        public string? OccupiedBy { get; set; }
        public int? OccupiedByUserId { get; set; }
        public int? CurrentOrderId { get; set; }
        public string? CurrentOrderStatus { get; set; }
        public bool? CurrentOrderIsPaid { get; set; }
        
        public CurrentOrderDto? CurrentOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CurrentOrderDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
    }
}
