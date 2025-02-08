using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    public required string ProductName { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Image { get; set; }
    public List<string> Colors { get; set; } = new List<string>();

    [Required(ErrorMessage = "Quantity is required")]
    public required int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public required decimal Price { get; set; }

    public string CategoryName { get; set; }
}