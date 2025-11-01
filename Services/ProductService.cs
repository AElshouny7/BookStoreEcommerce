
using BookStoreEcommerce.Models;
using System.Collections.Generic;
using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Services.Caching;

namespace BookStoreEcommerce.Services;

public class ProductService(
    StoreDbContext _context,
    IProductRepo _products,
    IMapper _mapper,
    ICacheService _cacheService
    ) : IProductService
{

    private readonly StoreDbContext _context = _context;
    private readonly IProductRepo _products = _products;
    private readonly IMapper _mapper = _mapper;
    private readonly ICacheService _cacheService = _cacheService;

    public async Task<IEnumerable<ProductReadDto>> GetAllProducts()
    {
        var cacheKey = CacheKeys.ProductsAll;

        var result = await _cacheService.GetOrCreateAsync(
            cacheKey,
            CacheTtls.ProductsList,

            async ct =>
            {
                var products = _products.GetAllProducts().ToList();
                return _mapper.Map<IEnumerable<ProductReadDto>>(products);
            }

        );

        await _cacheService.AddListKeyAsync(
            CacheKeys.ListKeysProductAllSet,
            cacheKey
        );

        return result!;
    }

    public async Task<IEnumerable<ProductReadDto>> GetAllProductsByCategory(int categoryId)
    {
        var cacheKey = CacheKeys.ProductsByCategory(categoryId);

        var result = await _cacheService.GetOrCreateAsync(
            cacheKey,
            CacheTtls.ProductsList,

            async ct =>
            {
                var products = _products.GetAllProductsByCategory(categoryId).ToList();
                return _mapper.Map<IEnumerable<ProductReadDto>>(products);
            }
        );

        await _cacheService.AddListKeyAsync(
            CacheKeys.ListKeysProductByCategoryAllSet,
            cacheKey
        );

        return result!;
    }

    public async Task<ProductReadDto?> GetProductById(int id)
    {
        var cacheKey = CacheKeys.ProductById(id);

        var result = await _cacheService.GetOrCreateAsync(
            cacheKey,
            CacheTtls.ProductDetail,
            async ct =>
            {
                var product = _products.GetProductById(id);
                return _mapper.Map<ProductReadDto>(product);
            }
        );

        return result;
    }

    public async Task<ProductReadDto> AddProduct(ProductCreateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        var createdProduct = _products.AddProduct(product);
        _context.SaveChanges();

        await InvalidateAfterProductChangeAsync(createdProduct.Id, createdProduct.CategoryId);

        return _mapper.Map<ProductReadDto>(createdProduct);
    }

    public async Task<ProductReadDto?> UpdateProduct(int id, ProductUpdateDto productDto)
    {
        var existingProduct = _products.GetProductById(id)
         ?? throw new InvalidOperationException("Product not found");

        var oldCategoryId = existingProduct.CategoryId;

        _mapper.Map(productDto, existingProduct);
        var updatedProduct = _products.UpdateProduct(existingProduct);
        _context.SaveChanges();

        if (oldCategoryId != updatedProduct.CategoryId)
            await InvalidateCategoryListAsync(oldCategoryId);

        await InvalidateAfterProductChangeAsync(updatedProduct.Id, updatedProduct.CategoryId);


        return _mapper.Map<ProductReadDto>(updatedProduct);
    }

    public async Task<ProductReadDto?> DeleteProduct(int id)
    {
        var product = _products.GetProductById(id)
        ?? throw new InvalidOperationException("Product not found");
        var deletedProduct = _products.DeleteProduct(id);
        _context.SaveChanges();

        await InvalidateAfterProductChangeAsync(deletedProduct.Id, deletedProduct.CategoryId);

        return _mapper.Map<ProductReadDto>(deletedProduct);
    }

    private async Task InvalidateAfterProductChangeAsync(int productId, int categoryId)
    {
        await _cacheService.RemoveAsync(CacheKeys.ProductById(productId));

        await _cacheService.InvalidateListAsync(CacheKeys.ListKeysProductAllSet);

        await _cacheService.RemoveAsync(CacheKeys.ProductsByCategory(categoryId));
    }

    private Task InvalidateCategoryListAsync(int categoryId)
    {
        return _cacheService.RemoveAsync(CacheKeys.ProductsByCategory(categoryId));
    }
}