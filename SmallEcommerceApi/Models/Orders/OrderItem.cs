using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmallEcommerceApi.Models.Products;

namespace SmallEcommerceApi.Models.Orders
{
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_variant_id")]
        public int ProductVariantId { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; } = null!;

        [Column("product_image")]
        public string? ProductImage { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
