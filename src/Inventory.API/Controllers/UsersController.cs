using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for user management operations (SuperAdmin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var result = await _userService.GetPagedAsync(pageNumber.Value, pageSize.Value, cancellationToken);
            return Ok(result);
        }
        
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        try
        {
            var user = await _userService.UpdateAsync(dto, cancellationToken);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the user", error = ex.Message });
        }
    }
}

