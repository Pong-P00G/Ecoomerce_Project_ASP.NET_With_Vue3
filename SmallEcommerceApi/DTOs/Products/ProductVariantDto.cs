

namespace SmallEcommerceApi.DTOs.Products
{
    public class ProductVariantDto
    {
        private static readonly List<int> list = new List<int>();

        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Variant options: { "Size": "Large", "Color": "Red" }
        public List<VariantOptionDto> Options { get; set; } = new List<VariantOptionDto>();
    }
}