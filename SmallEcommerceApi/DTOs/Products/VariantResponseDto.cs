namespace SmallEcommerceApi.DTOs.Products
{
    public class VariantResponseDto
    {
        public int ProductVariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<VariantOptionDto> Options { get; set; } = new List<VariantOptionDto>();
    }
}

