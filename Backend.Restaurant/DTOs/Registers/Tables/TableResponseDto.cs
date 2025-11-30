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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
