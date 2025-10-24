using BookStoreEcommerce.Dtos.Category;

namespace BookStoreEcommerce.Services
{
    public interface ICategoryService
    {
        IEnumerable<CategoryReadDto> GetAllCategories();
        CategoryReadDto? GetCategoryById(int id);
        CategoryReadDto AddCategory(CategoryCreateDto categoryDto);
        CategoryReadDto? UpdateCategory(int id, CategoryUpdateDto categoryDto);
        CategoryReadDto? DeleteCategory(int id);
    }
}