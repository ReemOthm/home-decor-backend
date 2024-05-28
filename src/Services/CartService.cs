using Microsoft.EntityFrameworkCore;
public class CartService
{
    private readonly AppDBContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public CartService(AppDBContext dbContext, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<Cart>?> GetCartItemsAsync(Guid userId)
    {
        var cartItems = await _dbContext.Carts
            .Where(c => c.UserID == userId)
            .Include(c => c.Products)
            .ToListAsync();

        return cartItems;
    }

    public async Task<bool> AddToCartAsync(Guid productId, Guid userId)
    {
        var existingCart = await _dbContext.Carts
            .Where(c => c.ProductID == productId && c.UserID == userId)
            .FirstOrDefaultAsync();

        // if (existingCart != null)
        // {
        //     // Product already exists in the user's cart
        //     return false;
        // }

        var newCart = new Cart
        {
            ProductID = productId,
            UserID = userId
        };

        _dbContext.Carts.Add(newCart);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromCartAsync(Guid productId, Guid userId)
    {
        var cartItem = await _dbContext.Carts
            .Where(c => c.ProductID == productId && c.UserID == userId)
            .FirstOrDefaultAsync();

        if (cartItem == null)
        {
            // Product not found in the user's cart
            return false;
        }

        _dbContext.Carts.Remove(cartItem);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ProductToRemoveFromCart(Guid userId, Guid productId)
    {
        var productToRemoveFromCart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.ProductID == productId && c.UserID == userId);
        if (productToRemoveFromCart != null)
        {
            _dbContext.Carts.Remove(productToRemoveFromCart);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
