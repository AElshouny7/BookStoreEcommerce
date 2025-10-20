namespace BookStoreEcommerce.Dtos.User
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserReadDto? User { get; set; }
    }
}