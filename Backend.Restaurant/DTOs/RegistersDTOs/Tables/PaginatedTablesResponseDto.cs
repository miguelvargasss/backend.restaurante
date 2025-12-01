namespace Backend.Restaurant.DTOs.Registers.Tables
{
    public class PaginatedTablesResponseDto
    {
        public List<TableResponseDto> Tables { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
