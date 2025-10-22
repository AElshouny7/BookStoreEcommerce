using BookStoreEcommerce.Dtos;
using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class OrderItemsProfile : AutoMapper.Profile
    {
        public OrderItemsProfile()
        {
            // // Client -> Domain (create item)
            // CreateMap<OrderItemCreateDto, OrderItems>();

            // // Domain -> Read DTO
            // CreateMap<OrderItems, OrderItemsReadDto>();
        }
    }
}
