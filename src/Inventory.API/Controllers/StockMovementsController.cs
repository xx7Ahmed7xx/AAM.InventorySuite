using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for stock movement operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockMovementsController(IStockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// Get all stock movements
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetAll(
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var result = await _stockService.GetPagedMovementsAsync(pageNumber.Value, pageSize.Value, cancellationToken);
            return Ok(result);
        }
        
        var movements = await _stockService.GetAllMovementsAsync(cancellationToken);
        return Ok(movements);
    }

    /// <summary>
    /// Get stock movements for a specific product
    /// </summary>
    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetByProduct(int productId, CancellationToken cancellationToken)
    {
        var movements = await _stockService.GetMovementsByProductIdAsync(productId, cancellationToken);
        return Ok(movements);
    }

    /// <summary>
    /// Get stock movements within a date range
    /// </summary>
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var movements = await _stockService.GetMovementsByDateRangeAsync(startDate, endDate, cancellationToken);
        return Ok(movements);
    }

    /// <summary>
    /// Add stock to a product
    /// </summary>
    [HttpPost("add")]
    public async Task<ActionResult<StockMovementDto>> AddStock([FromBody] StockMovementRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var movement = await _stockService.AddStockAsync(dto, cancellationToken);
            return Ok(movement);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove stock from a product
    /// </summary>
    [HttpPost("remove")]
    public async Task<ActionResult<StockMovementDto>> RemoveStock([FromBody] StockMovementRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var movement = await _stockService.RemoveStockAsync(dto, cancellationToken);
            return Ok(movement);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adjust stock for a product (correction)
    /// </summary>
    [HttpPost("adjust")]
    public async Task<ActionResult<StockMovementDto>> AdjustStock([FromBody] StockMovementRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var movement = await _stockService.AdjustStockAsync(dto, cancellationToken);
            return Ok(movement);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

