using System.Text.Json.Serialization;

namespace Inventory.Desktop.DTOs;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    
    // API expects Role as enum (number), but we work with string internally
    [JsonConverter(typeof(RoleToEnumConverter))]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
}
