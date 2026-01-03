using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models.Orders;
using SmallEcommerceApi.Models.Payments;

namespace SmallEcommerceApi.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrderController(AppDbContext db)
        {
            _db = db;
        }


        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);

            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}"
            };

            foreach (var item in cart.Items)
            {
                if (item.ProductVariant.StockQuantity < item.Quantity)
                    return BadRequest("Insufficient stock");

                item.ProductVariant.StockQuantity -= item.Quantity;

                order.Items.Add(new OrderItem
                {
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = item.Quantity * item.Price
                });

                order.Subtotal += item.Quantity * item.Price;
            }

            order.TotalAmount = order.Subtotal;

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cart.Items);

            await _db.SaveChangesAsync();
            return Ok(order.OrderNumber);
        }

        // Simple Payment API
        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment(int methodId, decimal amount)
        {
            var payment = new Payment
            {
                PaymentMethodId = methodId,
                Amount = amount,
                PaymentStatus = "PAID",
                TransactionId = Guid.NewGuid().ToString()
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return Ok(payment.PaymentId);
        }

    }
}
