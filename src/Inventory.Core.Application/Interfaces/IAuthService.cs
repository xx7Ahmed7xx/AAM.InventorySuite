using AAM.Inventory.Core.Application.DTOs;

namespace AAM.Inventory.Core.Application.Interfaces;

/// <summary>
/// Service interface for authentication
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

