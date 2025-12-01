namespace Backend.Restaurant.DTOs.RegistersDTOs.PaymentMethods
{
    public class PaginatedPaymentMethodsResponseDto
    {
        public List<PaymentMethodResponseDto> PaymentMethods { get; set; } = new List<PaymentMethodResponseDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
