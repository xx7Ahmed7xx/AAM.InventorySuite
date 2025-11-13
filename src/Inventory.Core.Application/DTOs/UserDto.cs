using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for User (without password)
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

