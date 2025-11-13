namespace AAM.Inventory.Core.Domain.Enums;

/// <summary>
/// User roles for authorization
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Cashier - Basic operations, limited access
    /// </summary>
    Cashier = 1,
    
    /// <summary>
    /// Moderator - Manage products, view reports
    /// </summary>
    Moderator = 2,
    
    /// <summary>
    /// Super Admin - Full access to all features
    /// </summary>
    SuperAdmin = 3
}

