//using Ecommerce_Project.DTOs.Products;
//using SmallEcommerceApi.Db;
//using SmallEcommerceApi.DTOs.Products;
//using SmallEcommerceApi.Models.Products;
//using SmallEcommerceApi.Services.Interfaces;

//namespace SmallEcommerceApi.Services
//{
//    public class ProductService(AppDbContext db, IWebHostEnvironment env) : IProductService
//    {
//        private readonly AppDbContext _db = db;
//        private readonly IWebHostEnvironment _env = env;

//        public async Task<int> CreateProductAsync(CreateProductDto dto)
//        {
//            var product = new Product
//            {
//                ProductName = dto.ProductName,
//                Description = dto.Description,
//                BasePrice = dto.BasePrice,
//                Sku = dto.Sku,
//                Featured = dto.Featured,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow
//            };

//            _db.Products.Add(product);
//            await _db.SaveChangesAsync(); // get ProductId

//            // Handle image
//            if (dto.ImageUrls != null)
//            {
//                var imagePath = await SaveImageAsync(dto.ImageUrls);

//                var image = new ProductImage
//                {
//                    ProductId = product.ProductId,
//                    ImageUrl = imagePath,
//                    IsPrimary = true,
//                    DisplayOrder = 0
//                };

//                _db.ProductImages.Add(image);
//                await _db.SaveChangesAsync();
//            }

//            return product.ProductId;
//        }

//        private async Task<string> SaveImageAsync(List<string> imageUrls)
//        {
//            throw new NotImplementedException();
//        }

//        private async Task<string> SaveImageAsync(IFormFile file)
//        {
//            var folder = Path.Combine(_env.WebRootPath, "uploads", "products");

//            if (!Directory.Exists(folder))
//                Directory.CreateDirectory(folder);

//            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//            var fullPath = Path.Combine(folder, fileName);

//            using var stream = new FileStream(fullPath, FileMode.Create);
//            await file.CopyToAsync(stream);

//            return $"/uploads/products/{fileName}";
//        }
//    }
//}