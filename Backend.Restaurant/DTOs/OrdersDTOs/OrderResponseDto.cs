namespace Backend.Restaurant.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        
        public int? UserId { get; set; }
        public int? TableId { get; set; }
        public string? TableName { get; set; }
        public int? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        
        public string? CustomerName { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public string? Observations { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
