using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.Category;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Services
{


    public class CategoryService(
     StoreDbContext _context,
     ICategoryRepo _categories,
     IMapper _mapper) : ICategoryService
    {

        private readonly StoreDbContext _context = _context;
        private readonly ICategoryRepo _categories = _categories;
        private readonly IMapper _mapper = _mapper;

        public async Task<IEnumerable<CategoryReadDto>> GetAllCategories()
        {
            var categories = await _categories.GetAllCategories();
            return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        }

        public CategoryReadDto? GetCategoryById(int id)
        {
            var category = _categories.GetCategoryById(id);
            return category is null ? null : _mapper.Map<CategoryReadDto?>(category);
        }

        public CategoryReadDto AddCategory(CategoryCreateDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var createdCategory = _categories.AddCategory(category);
            _context.SaveChanges();

            return _mapper.Map<CategoryReadDto>(createdCategory);
        }

        public CategoryReadDto? UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var existingCategory = _categories.GetCategoryById(id) ?? throw new InvalidOperationException("Category not found");
            _mapper.Map(categoryDto, existingCategory);
            var updatedCategory = _categories.UpdateCategory(existingCategory);
            _context.SaveChanges();

            return _mapper.Map<CategoryReadDto>(updatedCategory);
        }

        public CategoryReadDto? DeleteCategory(int id)
        {
            var category = _categories.GetCategoryById(id)
            ?? throw new InvalidOperationException("Category not found");
            var deletedCategory = _categories.DeleteCategory(id);
            _context.SaveChanges();

            return _mapper.Map<CategoryReadDto>(deletedCategory);
        }


    }

}