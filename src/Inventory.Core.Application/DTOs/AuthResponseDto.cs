using AAM.Inventory.Core.Domain.Enums;

namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
}

