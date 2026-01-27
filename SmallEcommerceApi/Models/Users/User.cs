using SmallEcommerceApi.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Users
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("username")]
        public string Username { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [NotMapped]
        [Column("first_name")]
        public string FirstName { get; set; } = null!;

        [Column("mid_name")]
        public string? MidName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; } = null!;

        [Column("full_name")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? FullName { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLogin { get; internal set; }

        public virtual UserRole? Role { get; set; }
        public virtual UserProfile? Profile { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
