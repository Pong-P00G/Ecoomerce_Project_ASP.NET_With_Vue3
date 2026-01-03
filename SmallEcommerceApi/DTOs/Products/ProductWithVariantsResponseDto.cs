namespace SmallEcommerceApi.DTOs.Products
{
    public class ProductWithVariantsResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? SKU { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public string? Supplier { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Featured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<CategoryDto> Categories { get; set; } = new();
        public List<ImageDto> Images { get; set; } = new();
        public List<VariantResponseDto> Variants { get; set; } = new();
    }
}
