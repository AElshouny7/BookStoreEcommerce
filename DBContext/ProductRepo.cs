using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public class ProductRepo(StoreDbContext context) : IProductRepo
    {
        public Product AddProduct(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            context.Products.Add(product);
            return product;
        }




        public Product? DeleteProduct(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(id));
            }
            var product = context.Products.FirstOrDefault(p => p.Id == id) ?? throw new InvalidOperationException("Product not found");
            context.Products.Remove(product);
            return product;

        }

        public IEnumerable<Product> GetAllProducts()
        {
            return context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return context.Products.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return context.SaveChanges() >= 0;

        }

        public Product? UpdateProduct(Product product)
        {
            // No implementation needed for EF Core as it tracks changes automatically
            return product;
        }
    }
}
