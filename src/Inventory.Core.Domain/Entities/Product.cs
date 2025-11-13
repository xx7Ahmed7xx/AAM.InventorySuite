namespace AAM.Inventory.Core.Domain.Entities;

/// <summary>
/// Represents a product in the inventory system.
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Stock Keeping Unit - unique identifier for the product
    /// </summary>
    public string SKU { get; set; } = string.Empty;
    
    /// <summary>
    /// Barcode for scanning
    /// </summary>
    public string? Barcode { get; set; }
    
    /// <summary>
    /// Selling price
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Cost price
    /// </summary>
    public decimal Cost { get; set; }
    
    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Minimum stock level for low-stock alerts
    /// </summary>
    public int MinimumStockLevel { get; set; }
    
    /// <summary>
    /// Category ID
    /// </summary>
    public int? CategoryId { get; set; }
    
    /// <summary>
    /// Navigation property to category
    /// </summary>
    public Category? Category { get; set; }
    
    /// <summary>
    /// Stock movements for this product
    /// </summary>
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    
    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Checks if the product is low on stock
    /// </summary>
    public bool IsLowStock => Quantity <= MinimumStockLevel;
}

