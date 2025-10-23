using System.Collections;
using BookStoreEcommerce.Dtos.Category;
using BookStoreEcommerce.Dtos.Product;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class ProductsProfile : AutoMapper.Profile
    {
        public ProductsProfile()
        {
            // Source -> TargetP
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(d => d.Price, o => o.PreCondition(src => src.Price.HasValue))
                .ForMember(d => d.StockQuantity, o => o.PreCondition(src => src.StockQuantity.HasValue))
                .ForMember(d => d.CategoryId, o => o.PreCondition(src => src.CategoryId.HasValue))
                .ForAllMembers(o => o.Condition((src, _, v) => v != null));

            CreateMap<Product, ProductReadDto>();
            CreateMap<Product, ProductListItemDto>();
            // CreateMap<Category, CategoryMiniDto>();
        }
    }

}