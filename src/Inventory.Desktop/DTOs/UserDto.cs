using System.Text.Json.Serialization;

namespace Inventory.Desktop.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // API returns role as number (enum), but we want it as string
    [JsonConverter(typeof(RoleConverter))]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

