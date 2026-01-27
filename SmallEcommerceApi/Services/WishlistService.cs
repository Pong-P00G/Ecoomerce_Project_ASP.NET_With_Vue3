using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models.Wishlists;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WishlistItem>> GetWishlistAsync(int userId)
        {
            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<WishlistItem> AddToWishlistAsync(int userId, int productId)
        {
            // Check if already in wishlist
            var existing = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing != null)
            {
                throw new InvalidOperationException("Product is already in your wishlist");
            }

            // Check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new ArgumentException("Product not found or unavailable");
            }

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            // Load relationships for response
            await _context.Entry(wishlistItem)
                .Reference(w => w.Product)
                .LoadAsync();

            return wishlistItem;
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
            {
                return false;
            }

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}
