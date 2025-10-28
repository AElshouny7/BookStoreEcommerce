using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos;
using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BookStoreEcommerce.Services;

public class OrderService
(
    StoreDbContext _context,
    IOrderRepo _orders,
    IProductRepo _products,
    IOrderItemsRepo _orderItems,
    IMapper _mapper
) : IOrderService
{
    public IEnumerable<OrderReadDto> GetAllOrders()
    {
        var orders = _orders.GetAllOrders()
            .OrderByDescending(order => order.OrderDate)
            .Select(order => order.Id)
            .ToList();

        foreach (var id in orders)
            yield return BuildOrderReadDto(id);
    }

    public OrderReadDto? GetOrderById(int id)
    {
        var order = _orders.GetOrderById(id);
        return order is null ? null : BuildOrderReadDto(id);
    }

    public OrderReadDto? CreateOrder(int userId, OrderCreateDto orderCreateDto)
    {
        if (orderCreateDto?.OrderItems is null || orderCreateDto.OrderItems.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one order item.");
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = Status.Pending,
            TotalAmount = 0m
        };

        _orders.AddOrder(order);
        _context.SaveChanges();

        decimal totalAmount = 0m;
        foreach (var itemDto in orderCreateDto.OrderItems)
        {
            var product = _products.GetProductById(itemDto.ProductId)
                 ?? throw new InvalidOperationException($"Product with ID {itemDto.ProductId} not found.");

            if (itemDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product ID {itemDto.ProductId}.");

            var unitPrice = product.Price;

            _orderItems.AddOrderItems(new OrderItems
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice
            });

            product.StockQuantity -= itemDto.Quantity;
            totalAmount += unitPrice * itemDto.Quantity;
        }

        order.TotalAmount = totalAmount;
        _context.SaveChanges();

        return BuildOrderReadDto(order.Id);

    }



    public IEnumerable<OrderReadDto> GetOrdersByUserId(int userId)
    {
        var orders = _orders.GetOrdersByUserId(userId)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => order.Id)
            .ToList();

        foreach (var id in orders)
            yield return BuildOrderReadDto(id);
    }


    public OrderReadDto? UpdateOrderStatus(int orderId, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status is required.");

        // Parse string → enum safely (case-insensitive)
        if (!Enum.TryParse<Status>(status, ignoreCase: true, out var parsed))
            throw new ArgumentException("Invalid status. Allowed: pending, completed, cancelled.");

        var order = _orders.GetOrderById(orderId);
        if (order is null) return null;

        order.Status = parsed;
        _orders.UpdateOrder(order);
        _context.SaveChanges();

        return BuildOrderReadDto(orderId);

    }

    public OrderReadDto? DeleteOrder(int orderId)
    {
        var order = _orders.GetOrderById(orderId);
        if (order is null) return null;

        var dtoBeforeDelete = BuildOrderReadDto(order.Id);

        var items = _orderItems.GetOrderItemsByOrderId(orderId).ToList();

        foreach (var i in items)
        {
            _orderItems.DeleteOrderItemsByOrderAndProductId(i.OrderId, i.ProductId);
        }

        _orders.DeleteOrder(orderId);
        _context.SaveChanges();

        return dtoBeforeDelete;
    }

    private OrderReadDto BuildOrderReadDto(int orderId)
    {
        var order = _orders.GetOrderById(orderId)!;

        var orderDto = _mapper.Map<OrderReadDto>(order); // maps Id, UserId, OrderDate, TotalAmount, Status->string

        var orderItems = _orderItems.GetOrderItemsByOrderId(orderId).ToList();
        var orderItemsDtos = _mapper.Map<List<OrderItemsReadDto>>(orderItems); // maps Quantity, UnitPrice, TotalPrice (via profile)

        var productIds = orderItems.Select(i => i.ProductId).Distinct().ToList();
        var productTitles = _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Title })
            .ToDictionary(p => p.Id, p => p.Title);

        foreach (var i in orderItemsDtos)
            i.ProductTitle = productTitles.TryGetValue(i.ProductId, out var t) ? t : null;

        orderDto.OrderItems = orderItemsDtos;
        return orderDto;
    }

}