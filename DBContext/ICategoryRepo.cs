using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{

    public interface ICategoryRepo
    {
        IEnumerable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        Category AddCategory(Category category);
        Category? UpdateCategory(Category category);
        Category? DeleteCategory(int id);
        bool SaveChanges();
    }
}