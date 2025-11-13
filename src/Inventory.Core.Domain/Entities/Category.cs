namespace AAM.Inventory.Core.Domain.Entities;

/// <summary>
/// Represents a product category.
/// </summary>
public class Category
{
    public int Id { get; set; }
    
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Products in this category
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
    /// <summary>
    /// Date and time when the category was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

