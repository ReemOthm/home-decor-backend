using api.Dtos;

public class AuthenticatedResponse
{
    public string? Token { get; set; }
    
    public string? RefreshToken { get; set; }

    public UserDto? Data { get; set; }
 
}