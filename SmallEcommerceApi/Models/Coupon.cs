using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models
{
    public class Coupon
    {
        [Key]
        [Column("coupon_id")]
        public int CouponId { get; set; }

        [Column("coupon_code")]
        public string CouponCode { get; set; } = null!;

        [Column("discount_type")]
        public string DiscountType { get; set; } = null!;

        [Column("discount_value")]
        public decimal DiscountValue { get; set; }

        [Column("min_order_amount")]
        public decimal? MinOrderAmount { get; set; }

        [Column("max_discount_amount")]
        public decimal? MaxDiscountAmount { get; set; }

        [Column("strat_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } public required string Name { get; set; }

        }
}
