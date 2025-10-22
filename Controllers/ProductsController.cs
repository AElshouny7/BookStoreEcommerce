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

    // // GET product by id
    // [HttpGet("{id}")]
    // public ActionResult<Product> GetProductById(int id)
    // {
    //     var product = ProductService.GetProductById(id);
    //     if (product == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(product);
    // }

    // POST create new product


    // PUT update product


    // DELETE delete product
}

