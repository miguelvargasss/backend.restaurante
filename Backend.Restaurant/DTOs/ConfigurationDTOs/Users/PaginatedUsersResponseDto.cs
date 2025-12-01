namespace Backend.Restaurant.DTOs.Configuration.Users
{
    public class PaginatedUsersResponseDto
    {
        public List<UserResponseDto> Users { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
