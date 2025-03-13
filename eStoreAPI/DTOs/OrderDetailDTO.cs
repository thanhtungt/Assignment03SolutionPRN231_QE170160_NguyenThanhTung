using BusinessObject.Models;
using System.Text.Json.Serialization;

public class OrderDetailDTO
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    [JsonIgnore]
    public virtual Order Order { get; set; } 
    [JsonIgnore]
    public virtual Product Product { get; set; } 
}
