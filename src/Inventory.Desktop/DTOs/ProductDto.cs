namespace Inventory.Desktop.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

