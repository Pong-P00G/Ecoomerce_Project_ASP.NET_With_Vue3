using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
