using SmallEcommerceApi.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models
{
    public class PasswordResetToken
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        public required string Token { get; set; }

        [Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public required User User { get; set; }
        public DateTime ExpiresAt { get; internal set; }
    }
}
