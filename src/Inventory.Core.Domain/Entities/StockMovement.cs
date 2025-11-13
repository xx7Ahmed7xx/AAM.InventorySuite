using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Domain.Entities;

/// <summary>
/// Represents a stock movement (add, remove, or adjustment).
/// </summary>
public class StockMovement
{
    public int Id { get; set; }
    
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }
    
    /// <summary>
    /// Navigation property to product
    /// </summary>
    public Product Product { get; set; } = null!;
    
    /// <summary>
    /// Type of movement (Add, Remove, Adjustment)
    /// </summary>
    public StockMovementType MovementType { get; set; }
    
    /// <summary>
    /// Quantity moved (positive for Add, negative for Remove, can be either for Adjustment)
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Reason for the movement
    /// </summary>
    public string? Reason { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// User who created the movement
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Date and time when the movement was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

