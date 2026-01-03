namespace SmallEcommerceApi.DTOs.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleId { get; set; } = "Customer"; // default as Customer
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; internal set; }
    }
}
