using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.User;

public class UserUpdateDto
{
    [StringLength(120)]
    public string? FullName { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}