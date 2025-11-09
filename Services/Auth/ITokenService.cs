using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Services.Auth
{
    public interface ITokenService
    {
        string CreateToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}