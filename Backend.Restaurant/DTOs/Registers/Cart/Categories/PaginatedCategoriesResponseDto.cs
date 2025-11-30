namespace Backend.Restaurant.DTOs.Registers.Cart.Categories
{
    public class PaginatedCategoriesResponseDto
    {
        public List<CategoryResponseDto> Categories { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
