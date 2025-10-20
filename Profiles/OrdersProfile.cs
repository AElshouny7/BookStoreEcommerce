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
            // Create: map ONLY order-level fields. Items are handled in the service
            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())     // set from auth
                .ForMember(d => d.OrderDate, o => o.Ignore())     // set in service
                .ForMember(d => d.TotalAmount, o => o.Ignore())     // compute in service
                .ForMember(d => d.Status, o => o.Ignore());    // set default in service

            // Read: enum -> string for Status; Items list is filled in service
            CreateMap<Order, OrderReadDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.OrderItems, o => o.Ignore());

            // Status update: string -> enum (ignore everything else)
            // CreateMap<OrderStatusUpdateDto, Order>()
            //     .ForAllOtherMembers(o => o.Ignore())
            //     .ForMember(d => d.Status, o =>
            //     {
            //         o.PreCondition(src => !string.IsNullOrWhiteSpace(src.Status));
            //         o.MapFrom(src =>
            //             Enum.TryParse<Status>(src.Status!, ignoreCase: true, out var parsed)
            //                 ? parsed
            //                 : throw new ArgumentException("Invalid status value"));
            //     });
        }
    }
}
