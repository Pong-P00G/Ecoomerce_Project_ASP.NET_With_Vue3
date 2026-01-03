using SmallEcommerceApi.Models;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(int userId);
        Task<Address?> GetByIdAsync(int id);
        Task<Address> CreateAsync(Address address);
        Task<Address?> UpdateAsync(int id, Address address);
        Task<bool> DeleteAsync(int id);
    }
}
