using AAM.Inventory.Core.Domain.Entities;

namespace AAM.Inventory.Core.Domain.Repositories;

/// <summary>
/// Repository interface for stock movement operations
/// </summary>
public interface IStockMovementRepository
{
    /// <summary>
    /// Get stock movement by ID
    /// </summary>
    Task<StockMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all stock movements
    /// </summary>
    Task<IEnumerable<StockMovement>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated stock movements
    /// </summary>
    Task<(IEnumerable<StockMovement> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get stock movements for a specific product
    /// </summary>
    Task<IEnumerable<StockMovement>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get stock movements within a date range
    /// </summary>
    Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new stock movement
    /// </summary>
    Task<StockMovement> AddAsync(StockMovement movement, CancellationToken cancellationToken = default);
}

