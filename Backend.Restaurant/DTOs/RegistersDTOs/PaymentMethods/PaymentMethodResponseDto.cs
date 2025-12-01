namespace Backend.Restaurant.DTOs.RegistersDTOs.PaymentMethods
{
    public class PaymentMethodResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool RequiresAuthorization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
