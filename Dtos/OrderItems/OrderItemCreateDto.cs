using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.OrderItems
{
    public class OrderItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}