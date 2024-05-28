namespace api.Dtos
{
    public class UserDto
    {
    public Guid UserID { get; set; } 
    public required string Username { get; set; }
    public required string Email { get; set; }

    // public required string Password { get; set; }
    
    public required string FirstName { get; set; }

    public required string LastName { get; set; } 

    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false; 
    public bool IsBanned { get; set; } = false; 
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public List<OrderModel> Orders { get; set; } = new List<OrderModel>();

    public List<CartModel> Carts { get; set; } = new List<CartModel>();
    }
}