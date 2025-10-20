namespace BookStoreEcommerce.Dtos
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string? Status { get; set; }
    }

}