using AAM.Inventory.Core.Application.DTOs;

namespace AAM.Inventory.Core.Application.Interfaces;

/// <summary>
/// Service interface for product operations
/// </summary>
public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

