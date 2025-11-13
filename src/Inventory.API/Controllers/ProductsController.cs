using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

/// <summary>
/// Controller for product management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
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
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Get product by SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductDto>> GetBySku(string sku, CancellationToken cancellationToken)
    {
        var product = await _productService.GetBySkuAsync(sku, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Get product by barcode
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductDto>> GetByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByBarcodeAsync(barcode, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term is required");
        }
        var products = await _productService.SearchAsync(term, cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId, CancellationToken cancellationToken)
    {
        var products = await _productService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Get products with low stock
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock(CancellationToken cancellationToken)
    {
        var products = await _productService.GetLowStockProductsAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product (Moderator and SuperAdmin only)
    /// </summary>
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing product (Moderator and SuperAdmin only)
    /// </summary>
    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var product = await _productService.UpdateAsync(dto, cancellationToken);
            return Ok(product);
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

    /// <summary>
    /// Delete a product (Moderator and SuperAdmin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Moderator,SuperAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

