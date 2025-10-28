using BookStoreEcommerce.Dtos;
using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class OrderItemsProfile : AutoMapper.Profile
    {
        public OrderItemsProfile()
        {
            // Client -> Domain
            CreateMap<OrderItemsReadDto, OrderItemsReadDto>()
                .ForMember(dest => dest.TotalPrice,
                    opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
                .ForMember(dest => dest.ProductTitle,
                    opt => opt.Ignore()); // needs a product-title dictionary (DB)
        }
    }
}
