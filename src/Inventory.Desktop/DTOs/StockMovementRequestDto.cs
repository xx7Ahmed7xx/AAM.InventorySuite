namespace Inventory.Desktop.DTOs;

public class StockMovementRequestDto
{
    public int ProductId { get; set; }
    public int MovementType { get; set; } // 1: Add, 2: Remove, 3: Adjustment
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
}

