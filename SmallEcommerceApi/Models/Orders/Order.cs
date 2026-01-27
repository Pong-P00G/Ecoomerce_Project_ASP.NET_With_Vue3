using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmallEcommerceApi.Models.Users;

namespace SmallEcommerceApi.Models.Orders
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("payment_id")]
        public int? PaymentId { get; set; }

        [Column("order_number")]
        public string OrderNumber { get; set; } = null!;

        [Column("order_status")]
        public string OrderStatus { get; set; } = "PENDING";

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = "card"; // card, qr, cash

        [Column("phone")]
        public string Phone { get; set; } = null!;

        [Column("shipping_address")]
        public string ShippingAddress { get; set; } = null!;

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("shipping_cost")]
        public decimal ShippingCost { get; set; }

        [Column("tax")]
        public decimal Tax { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
