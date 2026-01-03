using System.Linq;

namespace SmallEcommerceApi.Mapping
{
    internal static class RoleMapHelpers
    {
        private static readonly Dictionary<string, int> dictionary = new()
        {
            { "admin", 1 },
            { "customer", 2 }
        };

        public static Dictionary<string, int> RoleMap1 => dictionary;
    }
}