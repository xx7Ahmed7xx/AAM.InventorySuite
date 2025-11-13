using AAM.Inventory.Core.Domain.Entities;

namespace AAM.Inventory.Core.Domain.Repositories;

/// <summary>
/// Repository interface for product operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get product by SKU
    /// </summary>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get product by barcode
    /// </summary>
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all products
    /// </summary>
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated products
    /// </summary>
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get products by category
    /// </summary>
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get products with low stock
    /// </summary>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search products by name or SKU
    /// </summary>
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new product
    /// </summary>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing product
    /// </summary>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a product
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if SKU exists
    /// </summary>
    Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if barcode exists
    /// </summary>
    Task<bool> BarcodeExistsAsync(string barcode, int? excludeProductId = null, CancellationToken cancellationToken = default);
}

