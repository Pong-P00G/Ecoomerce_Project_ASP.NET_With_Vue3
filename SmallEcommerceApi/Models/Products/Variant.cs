using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Products
{
    public class Variant
    {
        [Key] public int VariantId { get; set; }
        [Required] public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<VariantOption> Options { get; set; } = new List<VariantOption>();
    }
}
