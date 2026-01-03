namespace SmallEcommerceApi.DTOs.Products
{
    public class CreateProductWithVariantDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? BasePrice { get; set; }
        public string? SKU { get; set; }

        // Stock fields for base product (optional)
        public int? Stock { get; set; }
        public int? MinStock { get; set; }
        public string? Supplier { get; set; }

        public bool IsActive { get; set; } = true;
        public bool Featured { get; set; } = false;

        public List<string>? CategoryNames { get; set; }
        public List<string>? ImageUrls { get; set; }

        public List<ProductVariantDto>? Variants { get; set; }
    }
}