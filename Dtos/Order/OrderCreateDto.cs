using System.ComponentModel.DataAnnotations;
using BookStoreEcommerce.Dtos.OrderItems;

namespace BookStoreEcommerce.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required]
        public List<OrderItemCreateDto>? OrderItems { get; set; }

    }
}