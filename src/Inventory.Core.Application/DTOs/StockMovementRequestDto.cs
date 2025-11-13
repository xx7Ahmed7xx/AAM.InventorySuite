using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// DTO for creating a stock movement
/// </summary>
public class StockMovementRequestDto
{
    public int ProductId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
}

