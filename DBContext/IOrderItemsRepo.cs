using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public interface IOrderItemsRepo
    {
        IEnumerable<OrderItems> GetAllOrderItems();
        IEnumerable<OrderItems> GetOrderItemsByOrderId(int orderId);
        IEnumerable<OrderItems> GetOrderItemsByProductId(int productId);
        OrderItems? GetOrderItemsByOrderAndProductId(int orderId, int productId);
        OrderItems? GetOrderItemById(int id);
        Task<OrderItems> AddOrderItems(OrderItems orderItems);
        OrderItems? UpdateOrderItems(OrderItems orderItems);
        OrderItems? DeleteOrderItemsByOrderAndProductId(int orderId, int productId);
        OrderItems? DeleteOrderItemsById(int id);
        bool SaveChanges();
    }
}