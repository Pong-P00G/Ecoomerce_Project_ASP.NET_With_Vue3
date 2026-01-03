namespace SmallEcommerceApi.DTOs.Products
{
    public class CreateProductVariantDto
    {
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        // IDs from variant_option table
        public List<int> OptionIds { get; set; } = new();
    }
}
