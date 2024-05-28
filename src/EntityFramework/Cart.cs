using System.ComponentModel.DataAnnotations;
public class Cart
{
    public Guid CartId = Guid.NewGuid();
    public Guid ProductID { get; set; }
    public Guid UserID { get; set; }
    public virtual Product? Product { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
    public virtual User? User { get; set; }
}
