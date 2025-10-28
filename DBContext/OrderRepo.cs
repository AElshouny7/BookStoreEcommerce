using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public class OrderRepo(StoreDbContext context) : IOrderRepo
    {
        private readonly StoreDbContext _context = context;


        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public IEnumerable<Order> GetOrdersByUserId(int userId)
        {
            var orders = _context.Orders.Where(o => o.UserId == userId).ToList();
            return orders;
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.Id == id);
        }

        public Order AddOrder(Order order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            _context.Orders.Add(order);
            return order;
        }

        public Order? UpdateOrder(Order order)
        {
            return order;
        }

        public Order? DeleteOrder(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid order ID", nameof(id));
            }
            var order = _context.Orders.FirstOrDefault(o => o.Id == id)
                ?? throw new InvalidOperationException("Order not found");
            _context.Orders.Remove(order);
            return order;
        }


    }
}