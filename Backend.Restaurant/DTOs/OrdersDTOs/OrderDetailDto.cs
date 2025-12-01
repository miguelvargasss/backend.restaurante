namespace Backend.Restaurant.DTOs.Orders
{
    public class OrderDetailDto
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string? Observations { get; set; }
        public string Status { get; set; } = "Pendiente";
    }
}
