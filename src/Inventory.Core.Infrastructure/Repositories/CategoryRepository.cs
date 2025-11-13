using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using AAM.Inventory.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AAM.Inventory.Core.Infrastructure.Repositories;

/// <summary>
/// Entity Framework implementation of ICategoryRepository
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly InventoryDbContext _context;

    public CategoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

