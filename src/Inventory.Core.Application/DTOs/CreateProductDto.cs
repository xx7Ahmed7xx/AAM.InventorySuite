namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int InitialQuantity { get; set; }
    public int MinimumStockLevel { get; set; }
    public int? CategoryId { get; set; }
}

