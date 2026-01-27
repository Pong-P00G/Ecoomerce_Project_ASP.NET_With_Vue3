using SmallEcommerceApi.Models.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Wishlists
{
    [Table("wishlist_item")]
    public class WishlistItem
    {
        [Key]
        [Column("wishlist_item_id")]
        public int WishlistItemId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; } = null!;
    }
}
