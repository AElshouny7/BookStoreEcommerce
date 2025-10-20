using BookStoreEcommerce.Dtos.Category;
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class ProductsProfile : AutoMapper.Profile
    {
        public ProductsProfile()
        {
            // Source -> Target
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opt => opt.Condition((src, _, val) => val != null));
            CreateMap<Product, ProductReadDto>();
            CreateMap<Product, ProductListItemDto>();
            CreateMap<Category, CategoryMiniDto>();
        }
    }
}