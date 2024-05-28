using System.Security.Claims;
using api.Controllers;
using api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{

    private readonly ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetAllProduct([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = )
    // {

    //     var product = await _productService.GetAllProductService(pageNumber, pageSize);
    //     if (product == null)
    //     {
    //         throw new NotFoundException("No Product Found");
    //     }
    //     return ApiResponse.Success(product, "All products are returned successfully");
    // }

    [HttpGet]
    public async Task<IActionResult> GetProducts(string? category,string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, bool isAscending, int page = 1, int pageSize = 6)
    {
        var products = await _productService.SearchProductsAsync(category,keyword, minPrice, maxPrice, sortBy, isAscending, page, pageSize);
        if (products != null)
        {
            return Ok(products);
        }
        else
        {
            throw new NotFoundException("No products found matching the search keyword");
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var product = await _productService.GetProductById(productIdGuid);
        if (product == null)
        {
            throw new NotFoundException("No Product Found");
        }
        else
        {
            return ApiResponse.Success(product, "single product is returned successfully");
        }
    }

    [HttpGet(":{slug}")]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var product = await _productService.GetProductBySlug(slug);
        if (product == null)
        {
            throw new NotFoundException("No Product Found");
        }
        else
        {
            return ApiResponse.Success(product, "single product is returned successfully");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductModel newProduct)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }        
        var response = await _productService.AddProductAsync(newProduct);
        return ApiResponse.Created(response);
    }

    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, ProductModel updateProduct)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }

        var result = await _productService.UpdateProductService(productId, updateProduct);
        if (!result)
        {
            throw new NotFoundException("product Not Found");
        }
        return ApiResponse.Updated("Product is Updated successfully");
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
                
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var result = await _productService.DeleteProductService(productIdGuid);
        if (!result)
        {
            throw new NotFoundException("No Product Found");
        }
        return ApiResponse.Deleted("product is Deleted successfully");
    }
}