namespace BookStoreEcommerce.Dtos.Product
{
    public class ProductQueryDto
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 4;
    }
}