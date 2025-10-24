
using BookStoreEcommerce.Models;
using System.Collections.Generic;
using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.Product;

namespace BookStoreEcommerce.Services
{
    public class ProductService(
        StoreDbContext _context,
        IProductRepo _products,
        IMapper _mapper) : IProductService
    {

        private readonly StoreDbContext _context = _context;
        private readonly IProductRepo _products = _products;
        private readonly IMapper _mapper = _mapper;

        public IEnumerable<ProductReadDto> GetAllProducts()
        {
            var products = _products.GetAllProducts();
            return _mapper.Map<IEnumerable<ProductReadDto>>(products);
        }

        public ProductReadDto? GetProductById(int id)
        {
            var p = _products.GetProductById(id) ?? throw new InvalidOperationException("Product not found");
            return _mapper.Map<ProductReadDto>(p);
        }

        public ProductReadDto AddProduct(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var createdProduct = _products.AddProduct(product);
            _context.SaveChanges();

            return _mapper.Map<ProductReadDto>(createdProduct);
        }

        public ProductReadDto? UpdateProduct(int id, ProductUpdateDto productDto)
        {
            var existingProduct = _products.GetProductById(id) ?? throw new InvalidOperationException("Product not found");
            _mapper.Map(productDto, existingProduct);
            var updatedProduct = _products.UpdateProduct(existingProduct);
            _context.SaveChanges();

            return _mapper.Map<ProductReadDto>(updatedProduct);
        }

        public ProductReadDto? DeleteProduct(int id)
        {
            var product = _products.GetProductById(id)
            ?? throw new InvalidOperationException("Product not found");
            var deletedProduct = _products.DeleteProduct(id);
            _context.SaveChanges();
            return _mapper.Map<ProductReadDto>(deletedProduct);
        }
    }
}

