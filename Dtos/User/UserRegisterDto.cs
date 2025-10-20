using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.User
{
    public class UserRegisterDto
    {
        [Required, StringLength(120)]
        public string? FullName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        public string? Password { get; set; }
    }
}