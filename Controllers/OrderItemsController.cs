using BookStoreEcommerce.Dtos.OrderItems;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api")]
public class OrderItemsController(IOrderItemsService orderItemsService) : ControllerBase
{
    private readonly IOrderItemsService _orderItemsService = orderItemsService;

    [HttpGet("orders/{orderId:int}/items")]
    public ActionResult<IEnumerable<OrderItemsReadDto>> GetOrderItemsByOrderId(int orderId)
        => Ok(_orderItemsService.GetOrderItemsByOrderId(orderId));

    [HttpGet("order-items/{id:int}")]
    public ActionResult<OrderItemsReadDto> GetOrderItemsById(int id)
    {
        var item = _orderItemsService.GetOrderItemsById(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("orders/{orderId:int}/items")]
    public ActionResult AddOrderItems(int orderId, [FromBody] OrderItemCreateDto dto)
    {
        try
        {
            var updatedOrder = _orderItemsService.AddOrderItems(orderId, dto);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("order-items/{orderItemId:int}")]
    public ActionResult UpdateQuantity(int orderItemId, [FromBody] OrderItemUpdateDto dto)
    {
        try
        {
            var updatedOrder = _orderItemsService.UpdateQuantity(orderItemId, dto);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("order-items/{orderItemId:int}")]
    public ActionResult RemoveOrderItemsById(int orderItemId)
    {
        try
        {
            var updatedOrder = _orderItemsService.RemoveOrderItemsById(orderItemId);
            return Ok(updatedOrder);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

}