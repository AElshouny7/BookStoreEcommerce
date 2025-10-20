namespace BookStoreEcommerce.Dtos.Product
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ImageURL { get; set; }
        public decimal Price { get; set; }
    }
}