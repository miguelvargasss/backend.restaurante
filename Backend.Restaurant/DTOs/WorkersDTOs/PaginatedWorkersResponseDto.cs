namespace Backend.Restaurant.DTOs.Workers
{
    public class PaginatedWorkersResponseDto
    {
        public List<WorkerResponseDto> Workers { get; set; } = new List<WorkerResponseDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
