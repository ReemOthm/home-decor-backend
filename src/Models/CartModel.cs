using System.ComponentModel.DataAnnotations;

public class CartModel
{

    [Required(ErrorMessage = "User Id is required")]
    public Guid ProductID { get; set; }
    [Required(ErrorMessage = "User Id is required")]
    public Guid UserID { get; set; }
    // public ProductModel? Product { get; set; } // ! uncomment this if there is any error
    public List<Product> Products { get; set; } = new List<Product>(); // ! comment this first if there is any errors
    public UserModel? User { get; set; }
}