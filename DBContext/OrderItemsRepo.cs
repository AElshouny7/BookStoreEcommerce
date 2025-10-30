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

        public OrderItems? GetOrderItemsByOrderAndProductId(int orderId, int productId)
        {
            return _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
        }

        public OrderItems? GetOrderItemById(int id)
        {
            return _context.OrderItems.FirstOrDefault(x => x.Id == id);
        }
        public OrderItems? UpdateOrderItems(OrderItems orderItems)
        {
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

        public OrderItems? DeleteOrderItemsById(int id)
        {
            var item = _context.OrderItems.FirstOrDefault(x => x.Id == id)
               ?? throw new InvalidOperationException("Order item not found.");

            _context.OrderItems.Remove(item);
            return item;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

    }
}