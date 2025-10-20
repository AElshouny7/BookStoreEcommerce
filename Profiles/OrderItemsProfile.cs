using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class OrderItemsProfile : AutoMapper.Profile
    {
        public OrderItemsProfile()
        {
            // Client -> Domain (create item)
            CreateMap<OrderItemCreateDto, OrderItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OrderId, o => o.Ignore())           // set when attaching to Order
                .ForMember(d => d.PriceAtPurchase, o => o.Ignore());  // set from Product.Price in service

            // Domain -> Read DTO
            CreateMap<OrderItem, OrderItemReadDto>()
                .ForMember(d => d.ProductTitle, o => o.MapFrom(s => s.Product.Title))
                .ForMember(d => d.UnitPriceAtPurchase, o => o.MapFrom(s => s.PriceAtPurchase))
                .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.PriceAtPurchase * s.Quantity));
        }
    }
}
