namespace Backend.Restaurant.DTOs.Registers.Feedback.Claims
{
    public class ClaimResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string Status { get; set; } = "Pendiente";
        public DateTime ClaimDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
