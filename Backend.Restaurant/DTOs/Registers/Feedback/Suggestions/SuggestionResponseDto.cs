namespace Backend.Restaurant.DTOs.Registers.Feedback.Suggestions
{
    public class SuggestionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string Status { get; set; } = "Pendiente";
        public DateTime SuggestionDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
