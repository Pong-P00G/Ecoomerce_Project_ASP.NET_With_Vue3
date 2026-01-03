using SmallEcommerceApi.DTOs.Products;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IProductService
    {
        Task<int> CreateProductAsync(CreateProductDto dto);
    }
}