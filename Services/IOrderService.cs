using BookStoreEcommerce.Dtos.Order;

namespace BookStoreEcommerce.Services;

public interface IOrderService
{
    IEnumerable<OrderReadDto> GetAllOrders();
    OrderReadDto? GetOrderById(int id);
    OrderReadDto? CreateOrder(int userId, OrderCreateDto orderCreateDto);
    OrderReadDto? DeleteOrder(int orderId);
    IEnumerable<OrderReadDto> GetOrdersByUserId(int userId);

    OrderReadDto UpdateOrderStatus(int orderId, string status);

}
