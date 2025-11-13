using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AAM.Inventory.Core.Application.Services;

/// <summary>
/// Service implementation for user management operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<PagedResultDto<UserDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _userRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return new PagedResultDto<UserDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user: {Username}", dto.Username);

        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogWarning("Attempted to create user with existing username: {Username}", dto.Username);
            throw new InvalidOperationException($"Username '{dto.Username}' already exists.");
        }

        // Check if email already exists
        var existingEmail = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingEmail != null)
        {
            _logger.LogWarning("Attempted to create user with existing email: {Email}", dto.Email);
            throw new InvalidOperationException($"Email '{dto.Email}' already exists.");
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = AuthService.HashPassword(dto.Password),
            Role = dto.Role,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
        _logger.LogInformation("User created successfully. ID: {UserId}, Username: {Username}", createdUser.Id, createdUser.Username);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {dto.Id} not found.");
        }

        // Check if username is being changed and if it already exists
        if (user.Username != dto.Username)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
            if (existingUser != null && existingUser.Id != dto.Id)
            {
                throw new InvalidOperationException($"Username '{dto.Username}' already exists.");
            }
        }

        // Check if email is being changed and if it already exists
        if (user.Email != dto.Email)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (existingEmail != null && existingEmail.Id != dto.Id)
            {
                throw new InvalidOperationException($"Email '{dto.Email}' already exists.");
            }
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Only update password if provided
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = AuthService.HashPassword(dto.Password);
        }

        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
        return MapToDto(updatedUser);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepository.DeleteAsync(id, cancellationToken);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            LastLoginDate = user.LastLoginDate,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

