
using BookStoreEcommerce.Dtos.Product;

namespace BookStoreEcommerce.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductReadDto>> GetAllProducts();
        Task<IEnumerable<ProductReadDto>> GetAllProductsByCategory(int categoryId);
        Task<ProductReadDto?> GetProductById(int id);
        Task<ProductReadDto> AddProduct(ProductCreateDto productDto);
        Task<ProductReadDto?> UpdateProduct(int id, ProductUpdateDto productDto);
        Task<ProductReadDto?> DeleteProduct(int id);

    }
}