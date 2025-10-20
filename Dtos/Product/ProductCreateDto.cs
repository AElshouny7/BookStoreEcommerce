using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.Product
{
    public class ProductCreateDto
    {
        [Required, StringLength(200)]
        public string? Title { get; set; }
        [Url]
        public string? ImageURL { get; set; }
        [Required, Range(0, 999999)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
