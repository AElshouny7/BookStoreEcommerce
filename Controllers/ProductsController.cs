
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;


    // GET all products 
    // TODO: add categories to filter
    [HttpGet]
    public ActionResult<IEnumerable<ProductReadDto>> GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    // GET product by id
    [HttpGet("{id:int}", Name = "GetProductById")]
    public ActionResult<ProductReadDto> GetProductById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // POST create new product
    [HttpPost]
    public ActionResult<ProductReadDto> AddProduct(ProductCreateDto productCreateDto)
    {
        try
        {
            var createdProduct = _productService.AddProduct(productCreateDto);

            return CreatedAtRoute(nameof(GetProductById), new { Id = createdProduct.Id }, createdProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }


    // PUT update product
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(int id, ProductUpdateDto productUpdateDto)
    {
        try
        {
            var updatedProduct = _productService.UpdateProduct(id, productUpdateDto);
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
    [HttpDelete("{id:int}")]
    public ActionResult<ProductReadDto> DeleteProduct(int id)
    {
        var deletedProduct = _productService.DeleteProduct(id);
        if (deletedProduct == null)
        {
            return NotFound();
        }
        return Ok(deletedProduct);
    }
}

