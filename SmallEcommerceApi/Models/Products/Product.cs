using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Products
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("product_name")]
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("base_price")]
        public decimal BasePrice { get; set; }

        [Column("sku")]
        [MaxLength(100)]
        public string? SKU { get; set; }

        [Column("Stock")]
        public int Stock { get; set; } = 0;

        [Column("Min_stock")]
        public int MinStock { get; set; } = 10;

        [Column("Supplier")]
        [MaxLength(200)]
        public string? Supplier { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("featured")]
        public bool Featured { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<ProductCategory> ProductCategories { get; set; }
            = new List<ProductCategory>();

        public ICollection<ProductImage> ProductImages { get; set; }
            = new List<ProductImage>();
        public ICollection<ProductVariant>? ProductVariants { get; set; }
            = new List<ProductVariant>();
    }
}