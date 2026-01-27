using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Users
{
    [Table("user_profile")]
    public class UserProfile
    {
        [Key]
        [Column("profile_id")]
        public int ProfileId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User? User { get; set; }
    }
}
