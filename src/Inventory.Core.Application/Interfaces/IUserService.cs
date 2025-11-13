using AAM.Inventory.Core.Application.DTOs;

namespace AAM.Inventory.Core.Application.Interfaces;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<UserDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

