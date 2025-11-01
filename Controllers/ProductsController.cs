
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;


    // GET all products
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProducts();
        return Ok(products);
    }

    // GET products by category
    [AllowAnonymous]
    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsByCategory(int categoryId)
    {
        var products = await _productService.GetAllProductsByCategory(categoryId);
        return Ok(products);
    }

    // GET product by id
    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<ActionResult<ProductReadDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // POST create new product
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> AddProduct(ProductCreateDto productCreateDto)
    {
        try
        {
            var createdProduct = await _productService.AddProduct(productCreateDto);

            return CreatedAtRoute(nameof(GetProductById), new { Id = createdProduct.Id }, createdProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }


    // PUT update product
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(int id, ProductUpdateDto productUpdateDto)
    {
        try
        {
            var updatedProduct = await _productService.UpdateProduct(id, productUpdateDto);
            if (updatedProduct == null)
            {
                return NotFound();
            }
            return Ok(updatedProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    // DELETE delete product
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> DeleteProduct(int id)
    {
        var deletedProduct = await _productService.DeleteProduct(id);
        if (deletedProduct == null)
        {
            return NotFound();
        }
        return Ok(deletedProduct);
    }
}

