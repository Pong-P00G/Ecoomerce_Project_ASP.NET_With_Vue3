using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Payments
{
    public record PaymentMethod([property: Key][property: Column("payment_method_id")] int PaymentMethodId, [property: Column("is_active")] bool IsActive)
    {
        public PaymentMethod(int paymentMethodId, string methodName, bool isActive) : this(paymentMethodId, isActive)
        {
            MethodName = methodName;
        }

        [Column("method_name")]
        public string MethodName { get; set; } = null!;
    }
}
