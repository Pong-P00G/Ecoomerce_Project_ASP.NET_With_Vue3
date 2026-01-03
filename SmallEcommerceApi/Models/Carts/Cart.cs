using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Carts
{
    public class Cart
    {
        [Key]
        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("session_id")]
        public string? SessionId { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
