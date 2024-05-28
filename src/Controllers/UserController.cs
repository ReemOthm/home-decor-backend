using System.Security.Claims;
using api.Controllers;
using api.Dtos;
using api.Dtos.User;
using api.Middlewares;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;
    private readonly AppDBContext _dbContext;


    public UserController(AppDBContext dbContext, UserService userService, AuthService authService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authService = authService;
        _dbContext = dbContext;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }

        var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);
        if (users == null)
        {
            throw new NotFoundException("No user Found");
        }
        return ApiResponse.Success(users, "all users are returned successfully");
    }

    [HttpGet("{userId}")]
    public IActionResult GetUser(Guid userId)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    // Singed in user only can get the information of their account
    [HttpGet("profile")]
    public IActionResult GetUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    [HttpPost("signup")]
    public async Task<IActionResult> CreateUser(UserModel newUser)
    {
        var createdUser = await _userService.CreateUser(newUser);
        if (createdUser != null)
        {
            return ApiResponse.Created("User is created successfully");
        }
        else
        {
            throw new Exception("An error occurred while creating the user.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("Invalid User Data");
        }
        var loggedInUser = await _userService.LoginUserAsync(loginDto);
        if (loggedInUser == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var refreshToken = _authService.GenerateRefreshToken();

        loggedInUser.RefreshToken = refreshToken;
        loggedInUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _dbContext.SaveChanges();

        var userDto = new UserDto
        {
            UserID = loggedInUser.UserID,
            Username = loggedInUser.Username,
            Email = loggedInUser.Email,
            FirstName = loggedInUser.FirstName,
            LastName = loggedInUser.LastName,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = loggedInUser.RefreshTokenExpiryTime,
            CreatedAt = loggedInUser.CreatedAt,
            Address = loggedInUser.Address,
            IsAdmin = loggedInUser.IsAdmin,
            IsBanned = loggedInUser.IsBanned,

        };

        var accessToken = _authService.GenerateJwt(userDto);


        return Ok(new AuthenticatedResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Data = userDto
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdatedUserDto updateUser)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = await _userService.UpdateUser(userId, updateUser);
        if (!user)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Updated("User is updated successfully");
    }

    [HttpPut("ban-unban")]
    public async Task<IActionResult> BanUnbaUser(string userIdString)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = await _userService.BannedUnBannedUser(userId);
        if (!user)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Updated("User is updated successfully");
    }


    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var result = await _userService.DeleteUser(userIdGuid);
        if (!result)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Deleted("User is deleted successfully");
    }
}
