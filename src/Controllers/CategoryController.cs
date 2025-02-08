using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using api.Controllers;
using api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategory()
    {
        var categories = await _categoryService.GetAllCategory();
        if (categories.ToList().Count < 1)
        {
            throw new NotFoundException("No Categories Found");
        }
        return ApiResponse.Success(categories, "all categories are returned successfully");
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCategoryPagination(int pageNumber = 1, int pageSize = 6)
    {
        var categories = await _categoryService.GetAllCategoryService(pageNumber, pageSize);
        if (categories == null)
        {
            throw new NotFoundException("No Categories Found");
        }
        return ApiResponse.Success(categories, "all categories are returned successfully");
    }


    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategory(Guid categoryId)
    {
        var category = await _categoryService.GetCategoryById(categoryId);
        if (category == null)
        {
            throw new NotFoundException("Category Not Found");
        }
        else
        {
            return ApiResponse.Success(category, "Category Found");
        }
    }

    [HttpGet("{productSlug}")]
    public async Task<IActionResult> GetCategoryByProductSlug(string productSlug)
    {
        var category = await _categoryService.GetCategoryByProductSlug(productSlug);
        if (category == null)
        {
            throw new NotFoundException("Category Not Found");
        }
        else
        {
            return ApiResponse.Success(category, "Category Found");
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryModel newCategory)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }

        var result = await _categoryService.CreateCategoryService(newCategory);
        if (!result)
        {
            throw new ValidationException("Invalid Category Data");
        }
        return ApiResponse.Created("Category is created successfully");
    }


    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryModel updateCategory)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        var result = await _categoryService.UpdateCategoryService(categoryId, updateCategory);
        if (!result)
        {
            throw new NotFoundException("Category Not Found");
        }
        return ApiResponse.Updated("Category is updated successfully");
    }


    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value;
        if (isAdmin != "Admin")
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }

        var result = await _categoryService.DeleteCategoryService(categoryId);
        if (!result)
        {
            throw new NotFoundException("Category Not Found");
        }
        return ApiResponse.Deleted("Category is deleted successfully");

    }
}
