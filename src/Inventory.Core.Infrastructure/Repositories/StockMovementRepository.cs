using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using AAM.Inventory.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AAM.Inventory.Core.Infrastructure.Repositories;

/// <summary>
/// Entity Framework implementation of IStockMovementRepository
/// </summary>
public class StockMovementRepository : IStockMovementRepository
{
    private readonly InventoryDbContext _context;

    public StockMovementRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(m => m.Product)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(m => m.Product)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<StockMovement> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.StockMovements
            .Include(m => m.Product)
            .OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<StockMovement>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(m => m.Product)
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(m => m.Product)
            .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockMovement> AddAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync(cancellationToken);
        return movement;
    }
}

