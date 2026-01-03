using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Carts;
using SmallEcommerceApi.Models.Carts;

namespace SmallEcommerceApi.Controllers.Carts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CartController(AppDbContext db)
        {
            _db = db;
        }

        private async Task<Cart> GetOrCreateCart(int? userId, string? sessionId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId)
                );

            if (cart != null) return cart;

            cart = new Cart
            {
                UserId = userId,
                SessionId = sessionId
            };

            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();

            return cart;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetCart(string? sessionId)
        {
            int? userId = User.Identity!.IsAuthenticated
                ? int.Parse(User.FindFirst("UserId")!.Value)
                : null;

            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId)
                );

            if (cart == null) return Ok(new { items = new List<object>() });

            return Ok(cart.Items.Select(i => new
            {
                i.CartItemId,
                i.ProductVariantId,
                i.Quantity,
                i.Price,
                Total = i.Quantity * i.Price
            }));
        }

        // POST: api/Cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            int? userId = User.Identity!.IsAuthenticated
                ? int.Parse(User.FindFirst("UserId")!.Value)
                : null;

            var cart = await GetOrCreateCart(userId, dto.SessionId);

            var variant = await _db.ProductVariants.FindAsync(dto.ProductVariantId);
            if (variant == null) return NotFound("Variant not found");

            var item = cart.Items.FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            if (item != null)
                item.Quantity += dto.Quantity;
            else
                _db.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    Price = variant.Price
                });

            await _db.SaveChangesAsync();
            return Ok();
        }

    }
}
