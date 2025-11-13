namespace AAM.Inventory.Core.Domain.Enums;

/// <summary>
/// Types of stock movements
/// </summary>
public enum StockMovementType
{
    /// <summary>
    /// Stock added (incoming)
    /// </summary>
    Add = 1,
    
    /// <summary>
    /// Stock removed (outgoing)
    /// </summary>
    Remove = 2,
    
    /// <summary>
    /// Stock adjustment (correction)
    /// </summary>
    Adjustment = 3
}

