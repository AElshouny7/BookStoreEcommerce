
using BookStoreEcommerce.Dtos.Product;

namespace BookStoreEcommerce.Services
{
    public interface IProductService
    {
        IEnumerable<ProductReadDto> GetAllProducts();
        ProductReadDto? GetProductById(int id);
        ProductReadDto AddProduct(ProductCreateDto productDto);
        ProductReadDto? UpdateProduct(int id, ProductUpdateDto productDto);
        ProductReadDto? DeleteProduct(int id);

    }
}