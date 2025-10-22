using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.Product
{
    public class ProductUpdateDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [Url]
        public string? ImageURL { get; set; }

        [Range(typeof(decimal), "0", "999999999.99")]
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        public int? CategoryId { get; set; }
    }
}
