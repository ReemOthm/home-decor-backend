using EntityFramework;
public class User
{
    public Guid UserID { get; set; } 

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string FirstName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public bool IsBanned { get; set; } = false;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public List<Order> Orders { get; set; } = new List<Order>();

    public List<Cart> Carts { get; set; } = new List<Cart>();
}
