using System.Security.Claims;
using api.Controllers;
using api.Middlewares;
using Microsoft.AspNetCore.Mvc;

[Route("api/")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("/account/cart")]
    public async Task<IActionResult> GetCartItems()
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
        var cartItems = await _cartService.GetCartItemsAsync(userId);

        if (cartItems != null)
        {
            return ApiResponse.Success(cartItems);
        }
        else
        {
            throw new Exception("Failed to fetch cart items.");
        }
    }

    [HttpPost("products/post/{productId}/add-to-cart")]
    public async Task<IActionResult> AddToCart(Guid productId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid user ID Format");
        }

        if (await _cartService.AddToCartAsync(productId, userId))
        {
            return ApiResponse.Created("Product added to cart successfully.");
        }
        else
        {
            throw new Exception("Failed to add product to cart.");
        }
    }

    [HttpDelete("account/cart/products/{productId:guid}/delete")]
    public async Task<IActionResult> DeleteProductFromCart(Guid productId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid user ID Format");
        }

        var result = await _cartService.ProductToRemoveFromCart(userId, productId);
        if (!result)
        {
            throw new NotFoundException("product does not exist in the cart");
        }
        return ApiResponse.Deleted("product is deleted from cart successfully");
    }
}
