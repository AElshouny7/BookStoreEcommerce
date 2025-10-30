using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Services;

public class OrderItemsService(
    StoreDbContext _context,
    IOrderRepo _orders,
    IOrderItemsRepo _orderItems,
    IProductRepo _products,
    IOrderService _orderService,   // reuse your hydrated order builder via GetOrderById
    IMapper _mapper
) : IOrderItemsService
{
    private readonly StoreDbContext _context = _context;
    private readonly IOrderRepo _orders = _orders;
    private readonly IOrderItemsRepo _orderItems = _orderItems;
    private readonly IProductRepo _products = _products;
    private readonly IOrderService _orderService = _orderService;
    private readonly IMapper _mapper = _mapper;

    public IEnumerable<OrderItemsReadDto> GetAllOrderItems()
    {
        var items = _orderItems.GetAllOrderItems().ToList();
        return HydrateTitles(_mapper.Map<List<OrderItemsReadDto>>(items), items);
    }

    public IEnumerable<OrderItemsReadDto> GetOrderItemsByOrderId(int orderId)
    {
        var items = _orderItems.GetOrderItemsByOrderId(orderId).ToList();
        return HydrateTitles(_mapper.Map<List<OrderItemsReadDto>>(items), items);
    }

    public IEnumerable<OrderItemsReadDto> GetOrderItemsByProductId(int productId)
    {
        var items = _orderItems.GetOrderItemsByProductId(productId).ToList();
        return HydrateTitles(_mapper.Map<List<OrderItemsReadDto>>(items), items);
    }

    public OrderItemsReadDto? GetOrderItemsByOrderAndProductId(int orderId, int productId)
    {
        var item = _orderItems.GetOrderItemsByOrderAndProductId(orderId, productId);
        if (item is null) return null;

        var dto = _mapper.Map<OrderItemsReadDto>(item);
        dto.ProductTitle = _context.Products
            .Where(p => p.Id == item.ProductId)
            .Select(p => p.Title)
            .FirstOrDefault();
        return dto;
    }

    public OrderItemsReadDto? GetOrderItemsById(int id)
    {
        var item = _orderItems.GetOrderItemById(id);
        if (item is null) return null;

        var dto = _mapper.Map<OrderItemsReadDto>(item);
        dto.ProductTitle = _context.Products
            .Where(p => p.Id == item.ProductId)
            .Select(p => p.Title)
            .FirstOrDefault();
        return dto;
    }

    public OrderReadDto AddOrderItems(int orderId, OrderItemCreateDto dto)
    {
        if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be >= 1.");

        var order = _orders.GetOrderById(orderId) ?? throw new InvalidOperationException("Order not found.");
        var product = _products.GetProductById(dto.ProductId) ?? throw new InvalidOperationException("Product not found.");


        var existing = _orderItems.GetOrderItemsByOrderAndProductId(orderId, dto.ProductId);
        var unitPrice = product.Price;

        int deltaQty = dto.Quantity;

        if (product.StockQuantity < deltaQty)
            throw new InvalidOperationException("Insufficient stock.");

        if (existing is not null)
        {
            existing.Quantity += deltaQty;
            _orderItems.UpdateOrderItems(existing);
        }
        else
        {

            _orderItems.AddOrderItems(new OrderItems
            {
                OrderId = orderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice
            });
        }

        product.StockQuantity -= deltaQty;
        order.TotalAmount += unitPrice * deltaQty;

        _context.SaveChanges();

        return _orderService.GetOrderById(orderId)!;
    }

    public OrderReadDto UpdateOrderItems(int orderItemId, OrderItemUpdateDto dto)
    {
        if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be >= 1.");

        var item = _orderItems.GetOrderItemById(orderItemId)
         ?? throw new InvalidOperationException("Order item not found.");
        var order = _orders.GetOrderById(item.OrderId)
         ?? throw new InvalidOperationException("Order not found.");
        var product = _products.GetProductById(item.ProductId)
         ?? throw new InvalidOperationException("Product not found.");

        int delta = dto.Quantity - item.Quantity;
        if (delta > 0)
        {
            if (product.StockQuantity < delta)
                throw new InvalidOperationException("Insufficient stock to increase quantity.");
            product.StockQuantity -= delta;
        }
        else if (delta < 0)
        {
            product.StockQuantity += -delta;
        }

        order.TotalAmount += item.UnitPrice * delta;
        item.Quantity = dto.Quantity;

        _orderItems.UpdateOrderItems(item);
        _context.SaveChanges();

        return _orderService.GetOrderById(order.Id)!;
    }

    public OrderReadDto? DeleteOrderItemsByOrderAndProductId(int orderId, int productId)
    {
        var item = _orderItems.GetOrderItemsByOrderAndProductId(orderId, productId);
        if (item is null) return null;

        var order = _orders.GetOrderById(orderId) ?? throw new InvalidOperationException("Order not found.");
        var product = _products.GetProductById(productId) ?? throw new InvalidOperationException("Product not found.");

        product.StockQuantity += item.Quantity;
        order.TotalAmount -= item.UnitPrice * item.Quantity;

        _orderItems.DeleteOrderItemsByOrderAndProductId(orderId, productId);
        _context.SaveChanges();

        return _orderService.GetOrderById(order.Id)!;
    }

    public OrderReadDto DeleteOrderItemsById(int orderItemId)
    {
        var item = _orderItems.GetOrderItemById(orderItemId) ?? throw new InvalidOperationException("Order item not found.");
        var order = _orders.GetOrderById(item.OrderId) ?? throw new InvalidOperationException("Order not found.");
        var product = _products.GetProductById(item.ProductId) ?? throw new InvalidOperationException("Product not found.");

        product.StockQuantity += item.Quantity;
        order.TotalAmount -= item.UnitPrice * item.Quantity;

        _orderItems.DeleteOrderItemsById(orderItemId);
        _context.SaveChanges();

        return _orderService.GetOrderById(order.Id)!;
    }


    private IEnumerable<OrderItemsReadDto> HydrateTitles(List<OrderItemsReadDto> dtos, List<OrderItems> items)
    {
        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        if (productIds.Count == 0) return dtos;

        var titles = _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Title })
            .ToDictionary(p => p.Id, p => p.Title);

        foreach (var i in dtos)
            i.ProductTitle = titles.TryGetValue(i.ProductId, out var t) ? t : null;

        return dtos;
    }
}