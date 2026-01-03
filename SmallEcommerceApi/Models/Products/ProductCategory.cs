using System.ComponentModel.DataAnnotations.Schema;

namespace SmallEcommerceApi.Models.Products
{
    [Table("product_category")]
    public class ProductCategory
    {
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        public Product? Product { get; set; }
        public Category? Category { get; set; }
    }
}