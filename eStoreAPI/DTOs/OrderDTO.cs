using System.Text.Json.Serialization;

public class OrderDTO
{
    public int OrderId { get; set; }
    public string MemberId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public decimal Freight { get; set; }
    [JsonIgnore]
    public ICollection<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
}
