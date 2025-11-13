using Xunit;
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Enums;
using AAM.Inventory.Core.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Core.Tests.Services;

public class StockServiceTests
{
    private readonly Mock<IStockMovementRepository> _mockMovementRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<StockService>> _mockLogger;
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _mockMovementRepository = new Mock<IStockMovementRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<StockService>>();
        _stockService = new StockService(_mockMovementRepository.Object, _mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddStockAsync_AddsStock_WhenValid()
    {
        // Arrange
        var product = new Product 
        { 
            Id = 1, 
            Name = "Test Product", 
            SKU = "SKU001", 
            Quantity = 10 
        };

        var dto = new StockMovementRequestDto
        {
            ProductId = 1,
            Quantity = 5,
            MovementType = StockMovementType.Add
        };

        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMovementRepository.Setup(r => r.AddAsync(It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovement m, CancellationToken ct) => m);

        // Act
        var result = await _stockService.AddStockAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
        Assert.Equal(5, result.Quantity);
        _mockProductRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockMovementRepository.Verify(r => r.AddAsync(It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddStockAsync_ThrowsException_WhenQuantityIsZero()
    {
        // Arrange
        var dto = new StockMovementRequestDto
        {
            ProductId = 1,
            Quantity = 0,
            MovementType = StockMovementType.Add
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _stockService.AddStockAsync(dto));
    }

    [Fact]
    public async Task RemoveStockAsync_ThrowsException_WhenInsufficientStock()
    {
        // Arrange
        var product = new Product 
        { 
            Id = 1, 
            Name = "Test Product", 
            SKU = "SKU001", 
            Quantity = 5 
        };

        var dto = new StockMovementRequestDto
        {
            ProductId = 1,
            Quantity = 10,
            MovementType = StockMovementType.Remove
        };

        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _stockService.RemoveStockAsync(dto));
    }
}

