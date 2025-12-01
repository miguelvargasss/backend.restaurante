namespace Backend.Restaurant.DTOs.Registers.Lounges
{
    public class PaginatedLoungesResponseDto
    {
        public List<LoungeResponseDto> Lounges { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
