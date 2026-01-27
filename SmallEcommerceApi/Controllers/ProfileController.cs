using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallEcommerceApi.Security.Api.Security;
using SmallEcommerceApi.Services.Interfaces;
using System.Security.Claims;

namespace SmallEcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public ProfileController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypesCustom.UserId);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        // GET: api/profile/wishlist
        [HttpGet("wishlist")]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Please login to view your wishlist" });
                }

                var wishlistItems = await _wishlistService.GetWishlistAsync(userId.Value);

                // Map to frontend-friendly format
                var result = wishlistItems.Select(item => new
                {
                    id = item.WishlistItemId,
                    productId = item.ProductId,
                    name = item.Product.ProductName,
                    price = item.Product.BasePrice,
                    image = item.Product.ProductImages?.FirstOrDefault()?.ImageUrl,
                    stock = item.Product.Stock,
                    inStock = item.Product.Stock > 0,
                    category = item.Product.ProductCategories?.FirstOrDefault()?.Category?.CategoryName,
                    addedAt = item.AddedAt
                });

                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/profile/wishlist
        [HttpPost("wishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Please login to add items to your wishlist" });
                }

                var wishlistItem = await _wishlistService.AddToWishlistAsync(userId.Value, request.ProductId);

                return Ok(new
                {
                    message = "Product added to wishlist",
                    data = new
                    {
                        id = wishlistItem.WishlistItemId,
                        productId = wishlistItem.ProductId,
                        name = wishlistItem.Product?.ProductName,
                        addedAt = wishlistItem.AddedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/profile/wishlist/{productId}
        [HttpDelete("wishlist/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Please login to manage your wishlist" });
                }

                var removed = await _wishlistService.RemoveFromWishlistAsync(userId.Value, productId);

                if (!removed)
                {
                    return NotFound(new { message = "Product not found in your wishlist" });
                }

                return Ok(new { message = "Product removed from wishlist" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class AddToWishlistRequest
    {
        public int ProductId { get; set; }
    }
}
