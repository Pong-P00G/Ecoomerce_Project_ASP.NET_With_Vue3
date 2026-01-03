
namespace SmallEcommerceApi.Mapping
{
    public static class RoleMap
    {
        public static readonly Dictionary<string, int> Map = new()
        {
            { "admin", 1 },
            { "customer", 2 },
            { "Staff", 3 }
        };

        public static bool ContainsKey(string role)
            => Map.ContainsKey(role.ToLower());

        public static int GetRoleId(string role)
            => Map[role.ToLower()];
    }
}
