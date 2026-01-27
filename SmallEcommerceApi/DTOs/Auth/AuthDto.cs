using System.ComponentModel.DataAnnotations;

namespace SmallEcommerceApi.DTOs.Auth
{
    public class AuthDto
    {
        public class RegisterDto
        {
            [Required(ErrorMessage = "Username is required")]
            [MaxLength(100)]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            [MaxLength(255)]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            [MaxLength(100)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "First name is required")]
            [MaxLength(100)]
            public string FirstName { get; set; } = string.Empty;

            [MaxLength(100)]
            public string? MidName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [MaxLength(100)]
            public string LastName { get; set; } = string.Empty;

            public int RoleId { get; set; } = 2; // Default to regular user role
        }

        public class LoginDto
        {
            [Required(ErrorMessage = "Username or email is required")]
            public string UsernameOrEmail { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = string.Empty;
        }

        public class AuthResponseDto
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public UserDto User { get; set; } = new UserDto();
        }

        public class RefreshTokenRequestDto
        {
            [Required]
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}
