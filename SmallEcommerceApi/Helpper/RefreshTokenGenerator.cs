using System.Security.Cryptography;

namespace SmallEcommerceApi.Helpper
{
    public static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
