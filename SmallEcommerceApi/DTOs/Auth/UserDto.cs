using System.ComponentModel.DataAnnotations;

namespace SmallEcommerceApi.DTOs.Auth
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MidName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RoleName { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MidName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? MidName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
    public class UserProfileDto
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
    }
    public class UpdateUserProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        [RegularExpression("MALE|FEMALE|OTHER", ErrorMessage = "Gender must be MALE, FEMALE, or OTHER")]
        public string? Gender { get; set; }
        public string? Bio { get; set; }
    }
}
