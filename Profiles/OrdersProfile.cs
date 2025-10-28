using AutoMapper;
using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Models;
using System;

namespace BookStoreEcommerce.Profiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Order, OrderReadDto>()
                .ForMember(dest => dest.Status,
                     opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OrderItems,
                     opt => opt.Ignore()); // Handled separately depends on a separate query (no nav props)
        }
    }
}
