using SmallEcommerceApi.Models.Wishlists;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetWishlistAsync(int userId);
        Task<WishlistItem> AddToWishlistAsync(int userId, int productId);
        Task<bool> RemoveFromWishlistAsync(int userId, int productId);
        Task<bool> IsInWishlistAsync(int userId, int productId);
    }
}
