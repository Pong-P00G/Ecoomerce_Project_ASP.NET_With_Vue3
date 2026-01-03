using SmallEcommerceApi.Models;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<Coupon> CreateAsync(Coupon coupon);
        Task<Coupon?> UpdateAsync(int id, Coupon coupon);
        Task<bool> DeleteAsync(int id);
    }
}
