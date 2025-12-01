using System.Text.Json.Serialization;

namespace Backend.Restaurant.DTOs.Configuration.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [JsonPropertyName("profileId")]
        public int? ProfilId { get; set; }
        [JsonPropertyName("profileName")]
        public string? ProfilName { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
