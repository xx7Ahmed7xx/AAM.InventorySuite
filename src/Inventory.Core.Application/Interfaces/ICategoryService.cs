using AAM.Inventory.Core.Application.DTOs;

namespace AAM.Inventory.Core.Application.Interfaces;

/// <summary>
/// Service interface for category operations
/// </summary>
public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateAsync(CategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

