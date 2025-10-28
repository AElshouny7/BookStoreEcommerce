using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Dtos.OrderItems;

namespace BookStoreEcommerce.Services
{
    public interface IOrderItemsService
    {
        OrderReadDto AddItem(int orderId, OrderItemCreateDto dto);

        OrderReadDto UpdateItemQuantity(int orderItemId, OrderItemUpdateDto dto);

        OrderReadDto RemoveItem(int orderItemId);
    }
}
