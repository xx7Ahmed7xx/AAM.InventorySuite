namespace AAM.Inventory.Core.Application.DTOs;

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

