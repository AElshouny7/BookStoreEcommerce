using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Data
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
        bool SaveChanges();
    }
}