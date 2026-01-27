using SmallEcommerceApi.Security.Api.Security;
using System.Security.Claims;

namespace SmallEcommerceApi.Security
{
    public static class UserClaims
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypesCustom.UserId);
            return int.TryParse(userId, out var id) ? id : 0;
        }

        public static string GetRole(ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypesCustom.Role) ?? string.Empty;

        public static string GetEmail(ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypesCustom.Email) ?? string.Empty;
    }
}
