namespace SmallEcommerceApi.Settings
{
    public class JwtSettings
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpireMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;
    }
}