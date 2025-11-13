using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating an existing user
/// </summary>
public class UpdateUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } // Optional - only update if provided
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}

