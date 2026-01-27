using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Orders;
using SmallEcommerceApi.Models.Orders;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            // Get user's cart with items
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                            .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty or not found");
            }

            // Calculate totals
            decimal subtotal = cart.Items.Sum(item => item.Price * item.Quantity);
            decimal shippingCost = subtotal > 100 ? 0 : 15;
            decimal tax = subtotal * 0.08m;
            decimal total = subtotal + shippingCost + tax;

            // Generate unique order number
            string orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderNumber = orderNumber,
                OrderStatus = "PENDING",
                PaymentMethod = dto.PaymentMethod,
                Phone = dto.Phone,
                ShippingAddress = dto.Location,
                Subtotal = subtotal,
                ShippingCost = shippingCost,
                Tax = tax,
                TotalAmount = total,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items from cart items
            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.ProductVariant?.Product;
                var firstImage = product?.ProductImages?.FirstOrDefault()?.ImageUrl;

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductVariantId = cartItem.ProductVariantId,
                    ProductName = product?.ProductName ?? "Unknown Product",
                    ProductImage = firstImage,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price,
                    TotalPrice = cartItem.Price * cartItem.Quantity
                };

                _context.OrderItems.Add(orderItem);
            }

            // Clear the cart
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            // Return the created order
            return await GetOrderByIdAsync(userId, order.OrderId) 
                ?? throw new InvalidOperationException("Failed to retrieve created order");
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.OrderId == orderId)
                .Include(o => o.Items)
                .FirstOrDefaultAsync();

            return order == null ? null : MapToDto(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                Phone = order.Phone,
                ShippingAddress = order.ShippingAddress,
                Subtotal = order.Subtotal,
                ShippingCost = order.ShippingCost,
                Tax = order.Tax,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(item => new OrderItemResponseDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }
    }
}
