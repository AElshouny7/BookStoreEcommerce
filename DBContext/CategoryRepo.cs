
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public class CategoryRepo(StoreDbContext context) : ICategoryRepo
    {
        public Category AddCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            context.Categories.Add(category);
            context.SaveChanges();
            return category;
        }

        public Category? DeleteCategory(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid category ID", nameof(id));
            }
            var category = context.Categories.FirstOrDefault(c => c.Id == id) ?? throw new InvalidOperationException("Category not found");
            context.Categories.Remove(category);
            context.SaveChanges();
            return category;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return context.Categories.ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public bool SaveChanges()
        {
            return context.SaveChanges() >= 0;
        }

        public Category? UpdateCategory(Category category)
        {
            // No implementation needed for EF Core as it tracks changes automatically
            return category;
        }
    }
}