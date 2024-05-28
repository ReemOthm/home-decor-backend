using System.ComponentModel.DataAnnotations;

public class ProductModel
{
    public Guid ProductID { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50)]
    public required string ProductName { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public string Image { get; set; }
    public List<string> Colors { get; set; } = new List<string>();

    [Required(ErrorMessage = "Quantity is required")]
    public required int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public required decimal Price { get; set; }

    [Required(ErrorMessage = "CategoryId is required")]
    public required Guid CategoryID { get; set; }
    public CategoryModel? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}