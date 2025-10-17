using System.ComponentModel.DataAnnotations;

namespace Models;

public class Order
{
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    [EnumDataType(typeof(Status))]
    public Status Status { get; set; }

}