//using Microsoft.EntityFrameworkCore;
//using SmallEcommerceApi.Db;
//using SmallEcommerceApi.Models;
//using SmallEcommerceApi.Services.Interfaces;

//namespace SmallEcommerceApi.Services
//{
//    public class CouponService : ICouponService
//    {
//        private readonly AppDbContext _db;
//        public CouponService(AppDbContext db) => _db = db;

//        public async Task<Coupon?> GetByCodeAsync(string code) =>
//            await _db.Coupons.FirstOrDefaultAsync(c => c.CouponCode == code && c.IsActive);

//        public async Task<IEnumerable<Coupon>> GetAllAsync() =>
//            await _db.Coupons.ToListAsync();

//        public async Task<Coupon> CreateAsync(Coupon coupon)
//        {
//            _db.Coupons.Add(coupon);
//            await _db.SaveChangesAsync();
//            return coupon;
//        }

//        public async Task<Coupon?> UpdateAsync(int id, Coupon coupon)
//        {
//            var existing = await _db.Coupons.FindAsync(id);
//            if (existing == null) return null;

//            existing.CouponCode = coupon.CouponCode;
//            existing.DiscountType = coupon.DiscountType;
//            existing.DiscountValue = coupon.DiscountValue;
//            existing.MinOrderAmount = coupon.MinOrderAmount;
//            existing.MaxDiscountAmount = coupon.MaxDiscountAmount;
//            existing.StartDate = coupon.StartDate;
//            existing.EndDate = coupon.EndDate;
//            existing.IsActive = coupon.IsActive;

//            await _db.SaveChangesAsync();
//            return existing;
//        }

//        public async Task<bool> DeleteAsync(int id)
//        {
//            var existing = await _db.Coupons.FindAsync(id);
//            if (existing == null) return false;

//            _db.Coupons.Remove(existing);
//            await _db.SaveChangesAsync();
//            return true;
//        }
//    }
//}
