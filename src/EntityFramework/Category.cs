
public class Category
{
    public Guid CategoryID { get; set; }

    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Product> Products { get; set; } = new List<Product>();
}
