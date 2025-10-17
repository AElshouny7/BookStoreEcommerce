using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Models;

public class OrderItems
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}