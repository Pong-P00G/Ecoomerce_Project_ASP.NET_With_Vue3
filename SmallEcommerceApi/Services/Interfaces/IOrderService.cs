using SmallEcommerceApi.DTOs.Orders;
using SmallEcommerceApi.Models.Orders;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<List<OrderResponseDto>> GetOrdersAsync(int userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(int userId, int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}
