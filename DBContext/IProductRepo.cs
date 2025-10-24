using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts();
        Product? GetProductById(int id);
        Product AddProduct(Product product);
        Product? UpdateProduct(Product product);
        Product? DeleteProduct(int id);
        bool SaveChanges();
    }
}