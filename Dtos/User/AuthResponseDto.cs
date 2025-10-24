namespace BookStoreEcommerce.Dtos.User
{
    public class AuthResponseDto
    {
        public string? Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public UserReadDto? User { get; set; } = default!;
    }
}