using BookStoreEcommerce.Models;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    public ProductController()
    {
    }

    // GET all products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAllProducts()
    {
        var products = ProductService.GetAllProducts();
        return Ok(products);
    }

    // GET product by id
    [HttpGet("{id}")]
    public ActionResult<Product> GetProductById(int id)
    {
        var product = ProductService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // POST create new product


    // PUT update product


    // DELETE delete product
}

