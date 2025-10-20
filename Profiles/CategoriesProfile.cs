using BookStoreEcommerce.Dtos.Category;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class CategoriesProfile : AutoMapper.Profile
    {
        public CategoriesProfile()
        {
            // Source -> Target
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(opt => opt.Condition((src, _, val) => val != null));
            CreateMap<Category, CategoryReadDto>();
            CreateMap<Category, CategoryMiniDto>();
        }
    }
}