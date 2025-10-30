using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Dtos.OrderItems;

namespace BookStoreEcommerce.Services
{
    public interface IOrderItemsService
    {
        IEnumerable<OrderItemsReadDto> GetAllOrderItems();
        IEnumerable<OrderItemsReadDto> GetOrderItemsByOrderId(int orderId);
        IEnumerable<OrderItemsReadDto> GetOrderItemsByProductId(int productId);
        OrderItemsReadDto? GetOrderItemsByOrderAndProductId(int orderId, int productId);
        OrderItemsReadDto? GetOrderItemsById(int id);
        OrderReadDto AddOrderItems(int orderId, OrderItemCreateDto dto);
        OrderReadDto? UpdateOrderItems(int orderItemId, OrderItemUpdateDto dto);
        OrderReadDto? DeleteOrderItemsByOrderAndProductId(int orderId, int productId);
        OrderReadDto? DeleteOrderItemsById(int id);


    }

}
