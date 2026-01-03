using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmallEcommerceApi.Models.Users;

namespace SmallEcommerceApi.Models
{
    public class RefreshToken
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        public required string Token { get; set; }

        [Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }= DateTime.UtcNow;

        [Column("is_revoked")]
        public bool IsRevoked { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        // Navigation property
        public required User User { get; set; }
        public DateTime ExpiresAt { get; internal set; }
    }
}
