using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

public class ProductDTO
{
    
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int CategoryId { get; set; }  // Hoặc bạn có thể muốn bao gồm thông tin Category
    [JsonIgnore]
    public CategoryDTO Category { get; set; }
}
