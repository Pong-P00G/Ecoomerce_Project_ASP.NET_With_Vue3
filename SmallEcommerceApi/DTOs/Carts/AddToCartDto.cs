namespace SmallEcommerceApi.DTOs.Carts
{
    public class AddToCartDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string? SessionId { get; set; }
    }
}
