using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.User
{
    public class UserLoginDto
    {
        [Required, EmailAddress]
        public string? Email { get; set; } = default!;
        [Required]
        public string? Password { get; set; } = default!;
    }
}