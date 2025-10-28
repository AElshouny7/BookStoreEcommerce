using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.OrderItems;

public class OrderItemUpdateDto
{
    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

