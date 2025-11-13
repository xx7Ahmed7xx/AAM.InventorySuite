using AAM.Inventory.Core.Domain.Entities;

namespace AAM.Inventory.Core.Domain.Repositories;

/// <summary>
/// Repository interface for category operations
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all categories
    /// </summary>
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new category
    /// </summary>
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing category
    /// </summary>
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a category
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

