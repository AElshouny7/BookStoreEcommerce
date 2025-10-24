using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public interface IOrderRepo
    {
        IEnumerable<Order> GetAllOrders();
        Order? GetOrderById(int id);
        Order AddOrder(Order order);
        Order? UpdateOrder(Order order);
        Order? DeleteOrder(int id);
        bool SaveChanges();
    }
}