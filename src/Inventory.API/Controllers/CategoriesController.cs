using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for category management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category (Moderator and SuperAdmin only)
    /// </summary>
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto dto, CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update an existing category (Moderator and SuperAdmin only)
    /// </summary>
    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var category = await _categoryService.UpdateAsync(dto, cancellationToken);
            return Ok(category);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a category (Moderator and SuperAdmin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

