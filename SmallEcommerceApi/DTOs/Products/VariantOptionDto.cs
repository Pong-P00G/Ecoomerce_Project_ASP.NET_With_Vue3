namespace SmallEcommerceApi.DTOs.Products
{
    public class VariantOptionDto
    {
        // Example: "Color", "Size", "DPI"
        public string Variant { get; set; } = string.Empty;

        // Example: "Black", "XL", "1600"
        public string Value { get; set; } = string.Empty;
    }
}