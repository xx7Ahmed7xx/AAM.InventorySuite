using Xunit;
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Enums;
using AAM.Inventory.Core.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using AAM.Inventory.Core.Application.Interfaces;

namespace Inventory.Core.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@test.com", Role = UserRole.Cashier, IsActive = true },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", Role = UserRole.Moderator, IsActive = true }
        };

        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@test.com",
            Role = UserRole.SuperAdmin,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal(UserRole.SuperAdmin, result.Role);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesUser_WhenValid()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "password123",
            Role = UserRole.Cashier,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.GetByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, ct) => capturedUser = u)
            .ReturnsAsync((User u, CancellationToken ct) => u);

        // Act
        var result = await _userService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("newuser@test.com", result.Email);
        Assert.NotNull(capturedUser);
        Assert.NotEmpty(capturedUser.PasswordHash); // Password should be hashed
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenUsernameExists()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "existinguser",
            Email = "new@test.com",
            Password = "password123",
            Role = UserRole.Cashier
        };

        var existingUser = new User { Id = 1, Username = "existinguser" };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenValid()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Username = "olduser",
            Email = "old@test.com",
            Role = UserRole.Cashier,
            IsActive = true
        };

        var updateDto = new UpdateUserDto
        {
            Id = 1,
            Username = "newuser",
            Email = "new@test.com",
            Role = UserRole.Moderator,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.GetByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);

        // Act
        var result = await _userService.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("new@test.com", result.Email);
        Assert.Equal(UserRole.Moderator, result.Role);
    }

    [Fact]
    public async Task DeleteAsync_DeletesUser_WhenExists()
    {
        // Arrange
        var user = new User { Id = 1, Username = "todelete" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteAsync(1);

        // Assert
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteAsync(999));
    }
}

