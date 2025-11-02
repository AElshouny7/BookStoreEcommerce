using System.Security.Claims;
using BookStoreEcommerce.Dtos.Order;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService _orderService) : ControllerBase
{
    private readonly IOrderService _orderService = _orderService;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public ActionResult<IEnumerable<OrderReadDto>> GetAllOrders()
    {
        var orders = _orderService.GetAllOrders();
        return Ok(orders);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public ActionResult<OrderReadDto> GetOrderById(int id)
    {
        var order = _orderService.GetOrderById(id);
        return order is null ? NotFound() : Ok(order);
    }

    [Authorize]
    [HttpGet("by-user")]
    public ActionResult<IEnumerable<OrderReadDto>> GetOrderByUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Invalid token: missing user id." });

        var orders = _orderService.GetOrdersByUserId(userId);
        return Ok(orders);
    }

    [Authorize(Roles = "Self")]
    [HttpPost]
    public ActionResult<OrderReadDto> CreateOrder([FromBody] OrderCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { error = "Invalid token: missing user id." });

        try
        {
            var created = _orderService.CreateOrder(userId, dto);
            if (created is null) return BadRequest(new { error = "Failed to create order." });

            return CreatedAtAction(nameof(GetOrderById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id:int}/status")]
    public ActionResult<OrderReadDto> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (string.IsNullOrWhiteSpace(dto.Status)) return BadRequest(new { error = "Status is required." });

        try
        {
            var updated = _orderService.UpdateOrderStatus(id, dto.Status);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public ActionResult<OrderReadDto> DeleteOrder(int id)
    {
        var deleted = _orderService.DeleteOrder(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }

}