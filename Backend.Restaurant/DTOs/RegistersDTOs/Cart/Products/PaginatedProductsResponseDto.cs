namespace Backend.Restaurant.DTOs.Registers.Cart.Products
{
    public class PaginatedProductsResponseDto
    {
        public List<ProductResponseDto> Products { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
