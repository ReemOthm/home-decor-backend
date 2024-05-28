using System.Security.Claims;
using api.Dtos;
using api.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDBContext _dbContext;
    private readonly ITokenService _tokenService;

    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(AppDBContext dbContext, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _passwordHasher = passwordHasher;
    }

    [HttpPost, Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto is null)
        {
            return BadRequest("Invalid client request");
        }

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return NotFound("User has not found");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
            // new Claim(ClaimTypes.Role, user.IsBanned? "Banned" : "notBanned"),

        };
        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _dbContext.SaveChanges();

        var userDto = new UserDto
        {
            UserID = user.UserID,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            CreatedAt = user.CreatedAt,
            Address = user.Address,
            IsAdmin = user.IsAdmin,
            IsBanned = user.IsBanned,

        };

        return Ok(new AuthenticatedResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Data = userDto
        });
    }
}