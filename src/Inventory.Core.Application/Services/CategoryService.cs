using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AAM.Inventory.Core.Application.Services;

/// <summary>
/// Service implementation for category operations
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category == null ? null : MapToDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category: {CategoryName}", dto.Name);
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.AddAsync(category, cancellationToken);
        _logger.LogInformation("Category created successfully. ID: {CategoryId}, Name: {CategoryName}", createdCategory.Id, createdCategory.Name);
        return MapToDto(createdCategory);
    }

    public async Task<CategoryDto> UpdateAsync(CategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.Id} not found.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return MapToDto(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting category ID: {CategoryId}", id);
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category not found for deletion. ID: {CategoryId}", id);
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        // Check if category has products
        if (category.Products.Any())
        {
            _logger.LogWarning("Cannot delete category {CategoryName} (ID: {CategoryId}) because it has {ProductCount} associated products", 
                category.Name, category.Id, category.Products.Count);
            throw new InvalidOperationException($"Cannot delete category '{category.Name}' because it has associated products.");
        }

        await _categoryRepository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Category deleted successfully. ID: {CategoryId}", id);
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = category.Products?.Count ?? 0,
            CreatedAt = category.CreatedAt
        };
    }
}

