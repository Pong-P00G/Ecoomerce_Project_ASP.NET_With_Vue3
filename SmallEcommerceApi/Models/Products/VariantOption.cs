using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Products
{
    public class VariantOption
    {
        [Column("option_id")]
        public int OptionId { get; set; }

        [Column("product_variant_id")]
        public int VariantId { get; set; }

        public string OptionValue { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Variant Variant { get; set; } = null!;
        public ICollection<ProductVariantOption> ProductVariantOptions { get; set; } = new List<ProductVariantOption>();
    }

}