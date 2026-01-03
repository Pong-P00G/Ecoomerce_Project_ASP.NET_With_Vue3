using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Products
{
    public class ProductVariantOption
    {
        public int ProductVariantId { get; set; }
        
        public int OptionId { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
        public VariantOption VariantOption { get; set; } = null!;
    }
}
