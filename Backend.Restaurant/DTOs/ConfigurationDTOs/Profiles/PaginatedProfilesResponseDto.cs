namespace Backend.Restaurant.DTOs.Configuration.Profiles
{
    public class PaginatedProfilesResponseDto
    {
        public List<ProfileResponseDto> Profiles { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
