namespace BookStoreEcommerce.DBContext
{
    using BookStoreEcommerce.Models;

    public class OrderItemsRepo(StoreDbContext context) : IOrderItemsRepo
    {
        private readonly StoreDbContext _context = context;

        public OrderItems AddOrderItems(OrderItems orderItems)
        {
            ArgumentNullException.ThrowIfNull(orderItems);
            _context.OrderItems.Add(orderItems);
            return orderItems;
        }

        public OrderItems? DeleteOrderItemsByOrderAndProductId(int orderId, int productId)
        {
            var existingOrderItem = _context.OrderItems
                .FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
            if (existingOrderItem != null)
            {
                _context.OrderItems.Remove(existingOrderItem);
                return existingOrderItem;
            }
            return null;
        }

        public IEnumerable<OrderItems> GetAllOrderItems()
        {
            return _context.OrderItems.ToList();
        }

        public IEnumerable<OrderItems> GetOrderItemsByOrderId(int orderId)
        {
            return _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToList();
        }

        public IEnumerable<OrderItems> GetOrderItemsByProductId(int productId)
        {
            return _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .ToList();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public OrderItems? UpdateOrderItems(OrderItems orderItems)
        {
            return orderItems;
        }
    }
}