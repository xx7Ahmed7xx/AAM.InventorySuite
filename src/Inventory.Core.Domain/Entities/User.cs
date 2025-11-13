using AAM.Inventory.Core.Domain.Common;
using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Cashier;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
}

