using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetAllProductsByCategory(int categoryId);

        Product? GetProductById(int id);
        Product AddProduct(Product product);
        Product? UpdateProduct(Product product);
        Product? DeleteProduct(int id);
        bool SaveChanges();
    }
}