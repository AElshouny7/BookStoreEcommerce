using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class OrderItemsProfile : AutoMapper.Profile
    {
        public OrderItemsProfile()
        {
            // Client -> Domain
            CreateMap<OrderItems, OrderItemsReadDto>()
                .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.UnitPrice * s.Quantity))
                .ForMember(d => d.ProductTitle, o => o.Ignore()); 


            CreateMap<OrderItemsReadDto, OrderItemsReadDto>()
                .ForMember(dest => dest.TotalPrice,
                    opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
                .ForMember(dest => dest.ProductTitle,
                    opt => opt.Ignore()); // needs a product-title dictionary (DB)
        }
    }
}
