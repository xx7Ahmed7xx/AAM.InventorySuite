using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Enums;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AAM.Inventory.Core.Application.Services;

/// <summary>
/// Service implementation for stock management operations
/// </summary>
public class StockService : IStockService
{
    private readonly IStockMovementRepository _movementRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<StockService> _logger;

    public StockService(IStockMovementRepository movementRepository, IProductRepository productRepository, ILogger<StockService> logger)
    {
        _movementRepository = movementRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<StockMovementDto> AddStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding stock. ProductId: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);
        
        if (dto.Quantity <= 0)
        {
            _logger.LogWarning("Invalid quantity for stock addition. ProductId: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);
            throw new ArgumentException("Quantity must be greater than zero.", nameof(dto));
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found for stock addition. ProductId: {ProductId}", dto.ProductId);
            throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");
        }

        var movement = new StockMovement
        {
            ProductId = dto.ProductId,
            MovementType = StockMovementType.Add,
            Quantity = dto.Quantity,
            Reason = dto.Reason,
            Notes = dto.Notes,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Update product quantity
        var oldQuantity = product.Quantity;
        product.Quantity += dto.Quantity;
        product.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(product, cancellationToken);

        var createdMovement = await _movementRepository.AddAsync(movement, cancellationToken);
        _logger.LogInformation("Stock added successfully. ProductId: {ProductId}, Quantity: {Quantity}, OldQuantity: {OldQuantity}, NewQuantity: {NewQuantity}", 
            dto.ProductId, dto.Quantity, oldQuantity, product.Quantity);
        return MapToDto(createdMovement, product);
    }

    public async Task<StockMovementDto> RemoveStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing stock. ProductId: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);
        
        if (dto.Quantity <= 0)
        {
            _logger.LogWarning("Invalid quantity for stock removal. ProductId: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);
            throw new ArgumentException("Quantity must be greater than zero.", nameof(dto));
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found for stock removal. ProductId: {ProductId}", dto.ProductId);
            throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");
        }

        if (product.Quantity < dto.Quantity)
        {
            _logger.LogWarning("Insufficient stock for removal. ProductId: {ProductId}, Available: {Available}, Requested: {Requested}", 
                dto.ProductId, product.Quantity, dto.Quantity);
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Quantity}, Requested: {dto.Quantity}");
        }

        var movement = new StockMovement
        {
            ProductId = dto.ProductId,
            MovementType = StockMovementType.Remove,
            Quantity = -dto.Quantity, // Negative for removal
            Reason = dto.Reason,
            Notes = dto.Notes,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Update product quantity
        product.Quantity -= dto.Quantity;
        product.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(product, cancellationToken);

        var createdMovement = await _movementRepository.AddAsync(movement, cancellationToken);
        return MapToDto(createdMovement, product);
    }

    public async Task<StockMovementDto> AdjustStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative for adjustment. Use absolute value.", nameof(dto));
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");
        }

        // Calculate the difference (delta) for the movement record
        var oldQuantity = product.Quantity;
        var newQuantity = dto.Quantity; // Set to absolute value
        var quantityDelta = newQuantity - oldQuantity;

        var movement = new StockMovement
        {
            ProductId = dto.ProductId,
            MovementType = StockMovementType.Adjustment,
            Quantity = quantityDelta, // Store the delta in the movement record
            Reason = dto.Reason,
            Notes = dto.Notes,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Set product quantity to the absolute value specified
        product.Quantity = newQuantity;
        product.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(product, cancellationToken);

        var createdMovement = await _movementRepository.AddAsync(movement, cancellationToken);
        return MapToDto(createdMovement, product);
    }

    public async Task<IEnumerable<StockMovementDto>> GetMovementsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetByProductIdAsync(productId, cancellationToken);
        return movements.Select(m => MapToDto(m, m.Product));
    }

    public async Task<IEnumerable<StockMovementDto>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        return movements.Select(m => MapToDto(m, m.Product));
    }

    public async Task<IEnumerable<StockMovementDto>> GetAllMovementsAsync(CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetAllAsync(cancellationToken);
        return movements.Select(m => MapToDto(m, m.Product));
    }

    public async Task<PagedResultDto<StockMovementDto>> GetPagedMovementsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _movementRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return new PagedResultDto<StockMovementDto>
        {
            Items = items.Select(m => MapToDto(m, m.Product)),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private StockMovementDto MapToDto(StockMovement movement, Product product)
    {
        return new StockMovementDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = product.Name,
            ProductSku = product.SKU,
            MovementType = movement.MovementType,
            MovementTypeName = movement.MovementType.ToString(),
            Quantity = movement.Quantity,
            Reason = movement.Reason,
            Notes = movement.Notes,
            CreatedBy = movement.CreatedBy,
            CreatedAt = movement.CreatedAt
        };
    }
}

