namespace SmallEcommerceApi.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string PaymentMethod { get; set; } = "card"; // card, qr, cash
        public string Phone { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = null!;
    }
}
