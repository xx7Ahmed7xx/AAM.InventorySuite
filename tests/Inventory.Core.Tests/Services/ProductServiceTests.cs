using Xunit;
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Inventory.Core.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<ProductService>> _mockLogger;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ProductService>>();
            _productService = new ProductService(_mockProductRepository.Object, _mockCategoryRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product 1", SKU = "SKU001", Quantity = 10, Price = 19.99m },
                new Product { Id = 2, Name = "Test Product 2", SKU = "SKU002", Quantity = 20, Price = 29.99m }
            };

            _mockProductRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockProductRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenExists()
        {
            // Arrange
            var product = new Product 
            { 
                Id = 1, 
                Name = "Test Product", 
                SKU = "SKU001", 
                Quantity = 10, 
                Price = 19.99m 
            };

            _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.Name);
            _mockProductRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockProductRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

