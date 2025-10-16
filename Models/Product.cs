using System.ComponentModel.DataAnnotations;

namespace Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }
    public string? ImageURL { get; set; }
    [Required]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
}