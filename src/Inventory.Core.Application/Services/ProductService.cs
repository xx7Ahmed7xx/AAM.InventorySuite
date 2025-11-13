using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AAM.Inventory.Core.Application.Services;

/// <summary>
/// Service implementation for product operations
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetBySkuAsync(sku, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByBarcodeAsync(barcode, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return new PagedResultDto<ProductDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetLowStockProductsAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.SearchAsync(searchTerm, cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating product with SKU: {SKU}", dto.SKU);

        // Validate SKU uniqueness
        if (await _productRepository.SkuExistsAsync(dto.SKU, null, cancellationToken))
        {
            _logger.LogWarning("Attempted to create product with existing SKU: {SKU}", dto.SKU);
            throw new InvalidOperationException($"Product with SKU '{dto.SKU}' already exists.");
        }

        // Validate barcode uniqueness if provided
        if (!string.IsNullOrWhiteSpace(dto.Barcode) && 
            await _productRepository.BarcodeExistsAsync(dto.Barcode, null, cancellationToken))
        {
            _logger.LogWarning("Attempted to create product with existing barcode: {Barcode}", dto.Barcode);
            throw new InvalidOperationException($"Product with barcode '{dto.Barcode}' already exists.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            SKU = dto.SKU,
            Barcode = dto.Barcode,
            Price = dto.Price,
            Cost = dto.Cost,
            Quantity = dto.InitialQuantity,
            MinimumStockLevel = dto.MinimumStockLevel,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product, cancellationToken);
        _logger.LogInformation("Product created successfully. ID: {ProductId}, SKU: {SKU}", createdProduct.Id, createdProduct.SKU);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating product ID: {ProductId}", dto.Id);
        var product = await _productRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found for update. ID: {ProductId}", dto.Id);
            throw new KeyNotFoundException($"Product with ID {dto.Id} not found.");
        }

        // Validate SKU uniqueness (excluding current product)
        if (await _productRepository.SkuExistsAsync(dto.SKU, dto.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Product with SKU '{dto.SKU}' already exists.");
        }

        // Validate barcode uniqueness if provided
        if (!string.IsNullOrWhiteSpace(dto.Barcode) && 
            await _productRepository.BarcodeExistsAsync(dto.Barcode, dto.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Product with barcode '{dto.Barcode}' already exists.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.SKU = dto.SKU;
        product.Barcode = dto.Barcode;
        product.Price = dto.Price;
        product.Cost = dto.Cost;
        product.MinimumStockLevel = dto.MinimumStockLevel;
        product.CategoryId = dto.CategoryId;
        
        // Update quantity if provided
        if (dto.Quantity.HasValue)
        {
            product.Quantity = dto.Quantity.Value;
        }
        
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product, cancellationToken);
        return MapToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting product ID: {ProductId}", id);
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found for deletion. ID: {ProductId}", id);
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        await _productRepository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Product deleted successfully. ID: {ProductId}", id);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Price = product.Price,
            Cost = product.Cost,
            Quantity = product.Quantity,
            MinimumStockLevel = product.MinimumStockLevel,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            IsLowStock = product.IsLowStock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

