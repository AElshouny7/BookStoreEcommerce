using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api")]
public class OrderItemsController(IOrderItemsService orderItemsService) : ControllerBase
{
    private readonly IOrderItemsService _orderItemsService = orderItemsService;

    [HttpGet("order-items")]
    public ActionResult<IEnumerable<OrderItemsReadDto>> GetAllOrderItems()
        => Ok(_orderItemsService.GetAllOrderItems());

    [HttpGet("orders/{orderId:int}/items")]
    public ActionResult<IEnumerable<OrderItemsReadDto>> GetByOrder(int orderId)
    {
        return Ok(_orderItemsService.GetOrderItemsByOrderId(orderId));
    }

    [HttpGet("products/{productId:int}/order-items")]
    public ActionResult<IEnumerable<OrderItemsReadDto>> GetByProduct(int productId)
    {
        return Ok(_orderItemsService.GetOrderItemsByProductId(productId));
    }

    [HttpGet("orders/{orderId:int}/items/{productId:int}")]
    public ActionResult<OrderItemsReadDto> GetByOrderAndProduct(int orderId, int productId)
    {
        var item = _orderItemsService.GetOrderItemsByOrderAndProductId(orderId, productId);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("order-items/{id:int}")]
    public ActionResult<OrderItemsReadDto> GetOrderItemsById(int id)
    {
        var item = _orderItemsService.GetOrderItemsById(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("orders/{orderId:int}/items")]
    public ActionResult AddOrderItems(int orderId, [FromBody] OrderItemCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var updatedOrder = _orderItemsService.AddOrderItems(orderId, dto);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("order-items/{orderItemId:int}")]
    public ActionResult UpdateOrderItems(int orderItemId, [FromBody] OrderItemUpdateDto dto)
    {
        try
        {
            var updatedOrder = _orderItemsService.UpdateOrderItems(orderItemId, dto);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("orders/{orderId:int}/items/{productId:int}")]
    public ActionResult<OrderReadDto> DeleteOrderItemsByOrderAndProductId(int orderId, int productId)
    {
        var updatedOrder = _orderItemsService.DeleteOrderItemsByOrderAndProductId(orderId, productId);
        return updatedOrder is null ? NotFound() : Ok(updatedOrder);
    }


    [HttpDelete("order-items/{orderItemId:int}")]
    public ActionResult<OrderReadDto> DeleteOrderItemsById(int orderItemId)
    {
        var updatedOrder = _orderItemsService.DeleteOrderItemsById(orderItemId);
        return updatedOrder is null ? NotFound() : Ok(updatedOrder);
    }

}