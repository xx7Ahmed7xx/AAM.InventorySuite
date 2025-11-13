using System.IO;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Domain.Repositories;
using AAM.Inventory.Core.Infrastructure.Data;
using AAM.Inventory.Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AAM.Inventory.Core.Infrastructure;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the service collection
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration - resolve path relative to solution root
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=inventory.db";

        // If path contains "data", resolve it relative to solution root
        if (connectionString.Contains("data") && !Path.IsPathRooted(connectionString))
        {
            // Try to find solution root by looking for .sln file or going up from bin/Debug
            var currentDir = AppContext.BaseDirectory;
            var solutionRoot = currentDir;
            
            // Navigate up from bin/Debug/net8.0 or bin/Debug/net8.0-windows to solution root
            for (int i = 0; i < 5; i++)
            {
                var slnFile = Directory.GetFiles(solutionRoot, "*.sln", SearchOption.TopDirectoryOnly);
                if (slnFile.Length > 0)
                {
                    break;
                }
                solutionRoot = Path.GetDirectoryName(solutionRoot) ?? solutionRoot;
            }
            
            var dbPath = Path.Combine(solutionRoot, "data", "inventory.db");
            // Ensure directory exists
            var dbDir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            connectionString = $"Data Source={dbPath}";
        }

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register application services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations to the database
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        
        await context.Database.MigrateAsync();
    }
}

