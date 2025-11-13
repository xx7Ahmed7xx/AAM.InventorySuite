namespace Inventory.Desktop.DTOs;

public class StockMovementDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int MovementType { get; set; } // 1: Add, 2: Remove, 3: Adjustment
    public string MovementTypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

