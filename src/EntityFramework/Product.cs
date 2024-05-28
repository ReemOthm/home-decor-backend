using System.ComponentModel.DataAnnotations;
using EntityFramework;

public class Product
{
    public Guid ProductID { get; set; } = Guid.NewGuid();
    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;
    public string Image { get; set; }
    public List<string> Colors { get; set; } = new List<string>();

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? CartId { get; set; }

    public virtual Cart? Cart { set; get; }
    public List<Order> Orders { get; set; } = new List<Order>();
}
