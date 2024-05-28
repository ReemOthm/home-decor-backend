using api.Dtos;
using api.Dtos.User;
using api.Services;
using AutoMapper;
using Dtos.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDBContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly AuthService _authService;

    public UserService(AppDBContext dbContext, IPasswordHasher<User> passwordHasher, IMapper mapper, AuthService authService)
    {
        _passwordHasher = passwordHasher;
        _dbContext = dbContext;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<PaginationResult<UserDto>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 6)
    {
        var totalCount = _dbContext.Users.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var users = await _dbContext.Users.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).Select(user => _mapper.Map<UserDto>(user)).ToListAsync();

        return new PaginationResult<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<User> GetUserById(Guid userId)
    {
        var user = await _dbContext.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.UserID == userId);
        user.Password = null;
        return user;
    }

    public async Task<User> CreateUser(UserModel newUser)
    {
        User createUser = new User
        {
            Username = newUser.Username,
            Email = newUser.Email,
            Password = _passwordHasher.HashPassword(null, newUser.Password),
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            CreatedAt = DateTime.UtcNow,
            BirthDate = newUser.BirthDate,
            Address = newUser.Address,
            IsAdmin = newUser.IsAdmin,
        };

        _dbContext.Users.Add(createUser);

        await _dbContext.SaveChangesAsync();

        return createUser;
    }

    public async Task<bool> UpdateUser(Guid userId, [FromBody] UpdatedUserDto updateUser)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (existingUser != null && updateUser != null)
        {
            existingUser.FirstName = updateUser.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updateUser.LastName ?? existingUser.LastName;
            // existingUser.IsBanned = updateUser.IsBanned;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
    public async Task<bool> BannedUnBannedUser(Guid userId)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (existingUser != null)
        {
            if (existingUser.IsBanned)
            {
                existingUser.IsBanned = false;
            }
            else
            {
                existingUser.IsBanned = true;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteUser(Guid userId)
    {

        var userToDelete = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (userToDelete != null)
        {
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<User?> LoginUserAsync(LoginDto loginDto)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }

}
