using Xunit;
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Inventory.Core.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<CategoryService>> _mockLogger;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<CategoryService>>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", Description = "Electronic items" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel" }
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _categoryService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockCategoryRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategory_WhenExists()
    {
        // Arrange
        var category = new Category 
        { 
            Id = 1, 
            Name = "Electronics", 
            Description = "Electronic items" 
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Electronics", result.Name);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockCategoryRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatesCategory_WhenValid()
    {
        // Arrange
        var dto = new CategoryDto
        {
            Name = "New Category",
            Description = "Category description"
        };

        Category? capturedCategory = null;
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Callback<Category, CancellationToken>((c, ct) => capturedCategory = c)
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        // Act
        var result = await _categoryService.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
        Assert.NotNull(capturedCategory);
        Assert.Equal("New Category", capturedCategory.Name);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenCategoryHasProducts()
    {
        // Arrange
        var category = new Category 
        { 
            Id = 1, 
            Name = "Electronics",
            Products = new List<Product> { new Product { Id = 1, Name = "Test Product" } }
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoryService.DeleteAsync(1));
    }
}

