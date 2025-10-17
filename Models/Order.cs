using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    [EnumDataType(typeof(Status))]
    public Status Status { get; set; }

}