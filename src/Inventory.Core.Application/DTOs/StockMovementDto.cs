using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for Stock Movement
/// </summary>
public class StockMovementDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public StockMovementType MovementType { get; set; }
    public string MovementTypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

