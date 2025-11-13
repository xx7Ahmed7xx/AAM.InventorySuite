using AAM.Inventory.Core.Application.DTOs;

namespace AAM.Inventory.Core.Application.Interfaces;

/// <summary>
/// Service interface for stock management operations
/// </summary>
public interface IStockService
{
    Task<StockMovementDto> AddStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default);
    Task<StockMovementDto> RemoveStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default);
    Task<StockMovementDto> AdjustStockAsync(StockMovementRequestDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovementDto>> GetMovementsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovementDto>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockMovementDto>> GetAllMovementsAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<StockMovementDto>> GetPagedMovementsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

