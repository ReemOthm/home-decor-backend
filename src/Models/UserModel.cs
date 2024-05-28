using System.ComponentModel.DataAnnotations;

public class UserModel 
{
    public Guid UserID { get; set; } 

    [Required(ErrorMessage = "Username is required")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
    [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
    public required string Email { get; set; }


    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    public required string Password { get; set; }

    [Required(ErrorMessage = "First name is required")]
    
    public required string FirstName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false; 
    public bool IsBanned { get; set; } = false; 
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public List<OrderModel> Orders { get; set; } = new List<OrderModel>();

    public List<CartModel> Carts { get; set; } = new List<CartModel>();
}
