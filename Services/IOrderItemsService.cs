using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Dtos.OrderItems;

namespace BookStoreEcommerce.Services
{
    public interface IOrderItemsService
    {
        IEnumerable<OrderItemsReadDto> GetOrderItemsByOrderId(int orderId);
        OrderItemsReadDto? GetOrderItemsById(int id);
        OrderReadDto AddOrderItems(int orderId, OrderItemCreateDto dto);
        OrderReadDto UpdateQuantity(int orderItemId, OrderItemUpdateDto dto);
        OrderReadDto RemoveOrderItemsById(int orderItemId);

    }

}
