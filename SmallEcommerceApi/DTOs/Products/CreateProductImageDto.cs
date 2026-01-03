namespace SmallEcommerceApi.DTOs.Products
{
    public class CreateProductImageDto
    {
        public required string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }

}