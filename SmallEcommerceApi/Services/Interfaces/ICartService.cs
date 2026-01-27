using SmallEcommerceApi.DTOs.Carts;
using SmallEcommerceApi.Models.Carts;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int? userId, string? sessionId);
        Task<Cart> AddToCartAsync(int? userId, string? sessionId, AddToCartDto addToCartDto);
        Task<Cart> RemoveFromCartAsync(int? userId, string? sessionId, int cartItemId);
        Task<Cart> UpdateCartItemQuantityAsync(int? userId, string? sessionId, int cartItemId, int quantity);
        Task ClearCartAsync(int? userId, string? sessionId);
        Task MergeCartsAsync(int userId, string sessionId);
    }
}
