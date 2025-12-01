namespace Backend.Restaurant.DTOs.Orders
{
    public class PaginatedOrdersResponseDto
    {
        public List<OrderResponseDto> Orders { get; set; } = new List<OrderResponseDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
