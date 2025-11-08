namespace BookStoreEcommerce.Controllers;

using AutoMapper;
using BookStoreEcommerce.Dtos.Category;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    // GET all categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategories();
        return Ok(categories);
    }

    // GET category by id
    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public ActionResult<CategoryReadDto> GetCategoryById(int id)
    {
        var category = _categoryService.GetCategoryById(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    // POST create new category
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<CategoryReadDto> AddCategory(CategoryCreateDto categoryCreateDto)
    {
        try
        {
            var createdCategory = _categoryService.AddCategory(categoryCreateDto);

            return CreatedAtRoute(nameof(GetCategoryById), new { Id = createdCategory.Id }, createdCategory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT update category
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public ActionResult<CategoryReadDto> UpdateCategory(int id, CategoryUpdateDto categoryUpdateDto)
    {
        try
        {
            var updatedCategory = _categoryService.UpdateCategory(id, categoryUpdateDto);
            if (updatedCategory == null)
            {
                return NotFound();
            }
            return Ok(updatedCategory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE category
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult<CategoryReadDto> DeleteCategory(int id)
    {
        try
        {
            var deletedCategory = _categoryService.DeleteCategory(id);
            if (deletedCategory == null)
            {
                return NotFound();
            }
            return Ok(deletedCategory);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}