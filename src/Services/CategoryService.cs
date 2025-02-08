using AutoMapper;
using Dtos.Pagination;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private AppDBContext _appDbContext;
    private IMapper _mapper;
    public CategoryService(AppDBContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllCategory()
    {
        return await _appDbContext.Categories.Include(c => c.Products).ToListAsync();
    }
    public async Task<PaginationResult<CategoryModel>> GetAllCategoryService(int pageNumber = 1, int pageSize = 6)
    {
        var totalCount = _appDbContext.Categories.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var categories = await _appDbContext.Categories.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).Select(category => _mapper.Map<CategoryModel>(category)).ToListAsync();

        return new PaginationResult<CategoryModel>
        {
            Items = categories,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<Category?> GetCategoryById(Guid categoryId)
    {
        return await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == categoryId);
    }
    public async Task<Category?> GetCategoryByProductSlug(string productSlug)
    {
        var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Slug == productSlug);
        if (product != null)
        {
            return await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == product.CategoryId);
        }
        return null;
    }

    public async Task<bool> CreateCategoryService(CategoryModel newCategory)
    {
        Category category = new Category
        {
            Name = newCategory.Name,
            Description = newCategory.Description,
        };

        category.Slug = Helper.GenerateSlug(category.Name);

        await _appDbContext.Categories.AddAsync(category);
        await _appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCategoryService(Guid categoryId, CategoryModel updateCategory)
    {
        var existingCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (existingCategory != null)
        {
            existingCategory.Name = updateCategory.Name ?? existingCategory.Name;
            existingCategory.Description = updateCategory.Description ?? existingCategory.Description;
            existingCategory.Slug = Helper.GenerateSlug(existingCategory.Name);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;

    }

    public async Task<bool> DeleteCategoryService(Guid categoryId)
    {
        var categoryToRemove = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (categoryToRemove != null)
        {
            _appDbContext.Categories.Remove(categoryToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}