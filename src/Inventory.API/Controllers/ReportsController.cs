using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for report generation
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
public class ReportsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IStockService _stockService;

    public ReportsController(IProductService productService, IStockService stockService)
    {
        _productService = productService;
        _stockService = stockService;
    }

    /// <summary>
    /// Get current stock report
    /// </summary>
    [HttpGet("stock")]
    public async Task<ActionResult<object>> GetStockReport(
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var result = await _productService.GetPagedAsync(pageNumber.Value, pageSize.Value, cancellationToken);
            return Ok(result);
        }
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Get low stock alert report
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<object>> GetLowStockReport(
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        // For low stock, we need to get all and filter, then paginate
        // This is a simplified approach - in production, you'd want a paginated low stock query
        var allLowStock = await _productService.GetLowStockProductsAsync(cancellationToken);
        var lowStockList = allLowStock.ToList();
        
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var totalCount = lowStockList.Count;
            var skip = (pageNumber.Value - 1) * pageSize.Value;
            var items = lowStockList.Skip(skip).Take(pageSize.Value);
            
            return Ok(new PagedResultDto<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value,
            });
        }
        
        return Ok(lowStockList);
    }

    /// <summary>
    /// Get movement history report
    /// </summary>
    [HttpGet("movements")]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetMovementHistory(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Parse date strings if provided
            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var start))
            {
                // Parse as local date, then convert start of day to UTC
                // This ensures we capture all movements from the start date in the user's timezone
                var localStart = DateTime.SpecifyKind(start.Date, DateTimeKind.Local); // Start of day in local time
                parsedStartDate = localStart.ToUniversalTime(); // Convert to UTC
            }

            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var end))
            {
                // To get the end of the selected day in UTC:
                // 1. Take the selected date (e.g., 2025-11-09)
                // 2. Get the start of the NEXT day (e.g., 2025-11-10 00:00:00) in local time
                // 3. Convert that to UTC
                // 4. Subtract one tick to get the very last moment of the original day in UTC
                // This ensures we include all movements from the selected day, regardless of timezone
                var nextDayStart = DateTime.SpecifyKind(end.Date.AddDays(1), DateTimeKind.Local); // Start of next day in local time
                parsedEndDate = nextDayStart.ToUniversalTime().AddTicks(-1); // Convert to UTC, then subtract 1 tick
            }

            IEnumerable<StockMovementDto> movements;

            // If both dates are provided, use date range
            if (parsedStartDate.HasValue && parsedEndDate.HasValue)
            {
                movements = await _stockService.GetMovementsByDateRangeAsync(parsedStartDate.Value, parsedEndDate.Value, cancellationToken);
            }
            else
            {
                // Otherwise, return all movements
                movements = await _stockService.GetAllMovementsAsync(cancellationToken);
            }

            return Ok(movements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving movement history", error = ex.Message });
        }
    }
}

