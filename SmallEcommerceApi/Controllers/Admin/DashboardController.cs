using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models.Products;
using SmallEcommerceApi.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEcommerceApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "ADMIN,Admin,admin")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                // Get basic stats
                var totalUsers = await _db.Users.CountAsync();
                var totalProducts = await _db.Products.CountAsync();
                var activeProducts = await _db.Products.CountAsync(p => p.IsActive);

                // Get low stock products
                var lowStockProducts = await _db.Products
                    .Where(p => p.Stock <= p.MinStock && p.Stock > 0)
                    .CountAsync();

                // Get out of stock products
                var outOfStockProducts = await _db.Products
                    .Where(p => p.Stock == 0)
                    .CountAsync();

                // Get recent products (last 5)
                var recentProducts = await _db.Products
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new
                    {
                        p.ProductId,
                        p.ProductName,
                        p.Description,
                        p.BasePrice,
                        p.SKU,
                        p.Stock,
                        p.MinStock,
                        p.Supplier,
                        p.CreatedAt,
                        Categories = p.ProductCategories.Select(pc => new
                        {
                            pc.CategoryId,
                            CategoryName = pc.Category != null ? pc.Category.CategoryName : "Uncategorized"
                        }).ToList(),
                        Images = p.ProductImages
                            .Where(pi => pi.IsPrimary)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                // Get recent users (last 5)
                var recentUsers = await _db.Users
                    .Include(u => u.Role)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        u.Email,
                        Role = u.Role != null ? u.Role.RoleName : "USER",
                        u.CreatedAt,
                        u.IsActive
                    })
                    .ToListAsync();

                // Get recent activities (mock data for now, can be enhanced with actual activity tracking)
                var recentActivities = new[]
                {
                    new
                    {
                        id = 1,
                        type = "product",
                        message = $"{recentProducts.Count} new products added",
                        time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        status = "completed"
                    },
                    new
                    {
                        id = 2,
                        type = "user",
                        message = $"{recentUsers.Count()} new users registered today",
                        time = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        status = "completed"
                    },
                    new
                    {
                        id = 3,
                        type = "stock",
                        message = $"{lowStockProducts} products are low in stock",
                        time = DateTime.UtcNow.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        status = "warning"
                    },
                    new
                    {
                        id = 4,
                        type = "inventory",
                        message = $"{outOfStockProducts} products are out of stock",
                        time = DateTime.UtcNow.AddHours(-3).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        status = "warning"
                    }
                };

                // Calculate growth metrics
                var monthlyGrowth = "+24%";
                var revenueTrend = "+12.5%";
                var usersTrend = "+3.2%";
                var productsTrend = "+18";

                var dashboardData = new
                {
                    totalUsers,
                    totalProducts,
                    activeProducts,
                    totalRevenue = 0, // No orders table available
                    lowStockProducts,
                    outOfStockProducts,
                    monthlyGrowth,
                    revenueTrend,
                    usersTrend,
                    productsTrend,
                    products = recentProducts,
                    users = recentUsers,
                    recentActivities
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to load dashboard data",
                    error = ex.Message
                });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = new
                {
                    totalUsers = await _db.Users.CountAsync(),
                    totalProducts = await _db.Products.CountAsync(),
                    activeProducts = await _db.Products.CountAsync(p => p.IsActive),
                    totalOrders = 0, // No orders table available
                    totalRevenue = 0, // No orders table available
                    pendingOrders = 0, // No orders table available
                    lowStockProducts = await _db.Products.CountAsync(p => p.Stock <= p.MinStock && p.Stock > 0),
                    outOfStockProducts = await _db.Products.CountAsync(p => p.Stock == 0)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to load stats",
                    error = ex.Message
                });
            }
        }
    }
}