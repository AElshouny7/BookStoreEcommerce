using System.Threading.Tasks;
using AutoMapper;
using BookStoreEcommerce.Data;
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Models;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepo repository, IMapper mapper) : ControllerBase
{
    private readonly IProductRepo _repository = repository;
    private readonly IMapper _mapper = mapper;


    // GET all products
    [HttpGet]
    public ActionResult<IEnumerable<ProductReadDto>> GetAllProducts()
    {
        var products = _repository.GetAllProducts();


        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
    }

    // GET product by id
    [HttpGet("{id}", Name = "GetProductById")]
    public ActionResult<ProductReadDto> GetProductById(int id)
    {
        var product = _repository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<ProductReadDto>(product));
    }

    // POST create new product
    [HttpPost]
    public ActionResult<ProductReadDto> AddProduct(ProductCreateDto productCreateDto)
    {
        var productModel = _mapper.Map<Product>(productCreateDto);
        _repository.AddProduct(productModel);
        _repository.SaveChanges();

        var productReadDto = _mapper.Map<ProductReadDto>(productModel);

        return CreatedAtRoute(nameof(GetProductById), new { Id = productReadDto.Id }, productReadDto);
    }


    // PUT update product
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(int id, ProductUpdateDto productUpdateDto)
    {
        var productModelFromRepo = _repository.GetProductById(id);
        if (productModelFromRepo == null)
            return NotFound();

        _mapper.Map(productUpdateDto, productModelFromRepo);
        _repository.UpdateProduct(productModelFromRepo);
        _repository.SaveChanges();

        return Ok(_mapper.Map<ProductReadDto>(productModelFromRepo));
    }


    // DELETE delete product
    [HttpDelete("{id}")]
    public ActionResult<ProductReadDto> DeleteProduct(int id)
    {
        var productModelFromRepo = _repository.GetProductById(id);
        if (productModelFromRepo == null)
        {
            return NotFound();
        }

        _repository.DeleteProduct(id);
        _repository.SaveChanges();

        return Ok(_mapper.Map<ProductReadDto>(productModelFromRepo));
    }
}

