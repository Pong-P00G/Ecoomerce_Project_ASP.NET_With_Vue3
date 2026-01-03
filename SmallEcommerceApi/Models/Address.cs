using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models
{
    public class Address
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("address_type")]
        public string AddressType { get; set; } = null!;

        [Column("address_line1")]
        public string AddressLine1 { get; set; } = null!;

        [Column("address_line2")]
        public string? AddressLine2 { get; set; }

        [Column("city")]
        public string City { get; set; } = null!;

        [Column("state")]
        public string State { get; set; } = null!;

        [Column("postal_code")]
        public string PostalCode { get; set; } = null!;

        [Column("country")]
        public string Country { get; set; } = null!;

        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; internal set; }
    }
}
