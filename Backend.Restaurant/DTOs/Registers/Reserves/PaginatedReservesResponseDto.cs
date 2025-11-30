namespace Backend.Restaurant.DTOs.Registers.Reserves
{
    public class PaginatedReservesResponseDto
    {
        public List<ReserveResponseDto> Reserves { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
