using System.Text.Json.Serialization;

public class CategoryDTO
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    [JsonIgnore]
    public ICollection<ProductDTO> Products { get; set; } = new List<ProductDTO>();
}
