namespace Backend.Restaurant.DTOs.SmallBox
{
    public class PaginatedSmallBoxResponseDto
    {
        public List<SmallBoxResponseDto> SmallBoxes { get; set; } = new List<SmallBoxResponseDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
