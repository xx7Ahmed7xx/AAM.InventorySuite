using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login endpoint
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
        }
    }

    /// <summary>
    /// Validate token endpoint
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateToken([FromBody] string token, CancellationToken cancellationToken)
    {
        var isValid = await _authService.ValidateTokenAsync(token, cancellationToken);
        return Ok(isValid);
    }
}

