using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Payments
{
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = null!;

        [Column("transaction_id")]
        public string TransactionId { get; set; } = null!;

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
