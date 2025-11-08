using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }
    public string? ImageURL { get; set; }
    [Required]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; } = Array.Empty<byte>();
}