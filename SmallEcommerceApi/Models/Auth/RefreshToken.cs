using SmallEcommerceApi.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Auth
{
    public class RefreshToken
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
