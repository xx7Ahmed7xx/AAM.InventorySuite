namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// DTO for updating an existing product
/// </summary>
public class UpdateProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int? Quantity { get; set; } // Optional: if provided, updates quantity directly
    public int MinimumStockLevel { get; set; }
    public int? CategoryId { get; set; }
}

