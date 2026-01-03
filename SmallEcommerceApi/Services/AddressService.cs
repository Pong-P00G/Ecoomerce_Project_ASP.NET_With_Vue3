using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _db;
        public AddressService(AppDbContext db) => _db = db;

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(int userId) =>
            await _db.Addresses.Where(a => a.UserId == userId && a.IsActive).ToListAsync();

        public async Task<Address?> GetByIdAsync(int id) =>
            await _db.Addresses.FindAsync(id);

        public async Task<Address> CreateAsync(Address address)
        {
            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> UpdateAsync(int id, Address address)
        {
            var existing = await _db.Addresses.FindAsync(id);
            if (existing == null) return null;

            existing.AddressLine1 = address.AddressLine1;
            existing.AddressLine2 = address.AddressLine2;
            existing.City = address.City;
            existing.State = address.State;
            existing.Country = address.Country;
            existing.PostalCode = address.PostalCode;
            existing.PhoneNumber = address.PhoneNumber;
            existing.IsDefault = address.IsDefault;
            existing.IsActive = address.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Addresses.FindAsync(id);
            if (existing == null) return false;

            _db.Addresses.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
