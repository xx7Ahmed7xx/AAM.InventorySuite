using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Enums;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AAM.Inventory.Core.Infrastructure.Data;

/// <summary>
/// Seeds the database with initial data
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var stockMovementRepository = scope.ServiceProvider.GetRequiredService<IStockMovementRepository>();

        // Seed Users (one per role)
        await SeedUsersAsync(userRepository);

        // Seed Categories
        var categories = await SeedCategoriesAsync(categoryRepository);

        // Seed Products
        var products = await SeedProductsAsync(productRepository, categories);

        // Seed Stock Movements
        await SeedStockMovementsAsync(stockMovementRepository, products);
    }

    private static async Task SeedUsersAsync(IUserRepository userRepository)
    {
        var users = new[]
        {
            new User
            {
                Username = "admin",
                Email = "admin@inventory.com",
                PasswordHash = AuthService.HashPassword("admin123"),
                Role = UserRole.SuperAdmin,
                IsActive = true
            },
            new User
            {
                Username = "moderator",
                Email = "moderator@inventory.com",
                PasswordHash = AuthService.HashPassword("moderator123"),
                Role = UserRole.Moderator,
                IsActive = true
            },
            new User
            {
                Username = "cashier",
                Email = "cashier@inventory.com",
                PasswordHash = AuthService.HashPassword("cashier123"),
                Role = UserRole.Cashier,
                IsActive = true
            }
        };

        foreach (var user in users)
        {
            var existing = await userRepository.GetByUsernameAsync(user.Username);
            if (existing == null)
            {
                await userRepository.CreateAsync(user);
            }
        }
    }

    private static async Task<List<Category>> SeedCategoriesAsync(ICategoryRepository categoryRepository)
    {
        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices and components" },
            new Category { Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Name = "Food & Beverages", Description = "Food products and drinks" },
            new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" },
            new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" }
        };

        var createdCategories = new List<Category>();
        var existingCategories = (await categoryRepository.GetAllAsync()).ToList();
        
        foreach (var category in categories)
        {
            var existing = existingCategories.FirstOrDefault(c => c.Name == category.Name);
            if (existing == null)
            {
                var created = await categoryRepository.AddAsync(category);
                createdCategories.Add(created);
            }
            else
            {
                createdCategories.Add(existing);
            }
        }

        return createdCategories;
    }

    private static async Task<List<Product>> SeedProductsAsync(IProductRepository productRepository, List<Category> categories)
    {
        var random = new Random(42); // Fixed seed for reproducibility
        var products = new List<Product>();

        // Electronics (10 products)
        var electronics = categories.FirstOrDefault(c => c.Name == "Electronics");
        if (electronics != null)
        {
            products.AddRange(new[]
            {
                new Product { Name = "Laptop Pro 15", SKU = "ELEC-001", Barcode = "1234567890123", Price = 1299.99m, Cost = 800.00m, Quantity = 25, MinimumStockLevel = 10, CategoryId = electronics.Id },
                new Product { Name = "Wireless Mouse", SKU = "ELEC-002", Barcode = "1234567890124", Price = 29.99m, Cost = 12.00m, Quantity = 150, MinimumStockLevel = 50, CategoryId = electronics.Id },
                new Product { Name = "Mechanical Keyboard", SKU = "ELEC-003", Barcode = "1234567890125", Price = 89.99m, Cost = 45.00m, Quantity = 75, MinimumStockLevel = 20, CategoryId = electronics.Id },
                new Product { Name = "USB-C Hub", SKU = "ELEC-004", Barcode = "1234567890126", Price = 49.99m, Cost = 20.00m, Quantity = 100, MinimumStockLevel = 30, CategoryId = electronics.Id },
                new Product { Name = "Monitor 27\" 4K", SKU = "ELEC-005", Barcode = "1234567890127", Price = 399.99m, Cost = 250.00m, Quantity = 40, MinimumStockLevel = 15, CategoryId = electronics.Id },
                new Product { Name = "Webcam HD", SKU = "ELEC-006", Barcode = "1234567890128", Price = 79.99m, Cost = 35.00m, Quantity = 60, MinimumStockLevel = 20, CategoryId = electronics.Id },
                new Product { Name = "Bluetooth Speaker", SKU = "ELEC-007", Barcode = "1234567890129", Price = 59.99m, Cost = 25.00m, Quantity = 90, MinimumStockLevel = 30, CategoryId = electronics.Id },
                new Product { Name = "Tablet Stand", SKU = "ELEC-008", Barcode = "1234567890130", Price = 24.99m, Cost = 10.00m, Quantity = 120, MinimumStockLevel = 40, CategoryId = electronics.Id },
                new Product { Name = "Power Bank 20000mAh", SKU = "ELEC-009", Barcode = "1234567890131", Price = 39.99m, Cost = 18.00m, Quantity = 80, MinimumStockLevel = 25, CategoryId = electronics.Id },
                new Product { Name = "USB Flash Drive 64GB", SKU = "ELEC-010", Barcode = "1234567890132", Price = 12.99m, Cost = 6.00m, Quantity = 200, MinimumStockLevel = 50, CategoryId = electronics.Id }
            });
        }

        // Clothing (10 products)
        var clothing = categories.FirstOrDefault(c => c.Name == "Clothing");
        if (clothing != null)
        {
            products.AddRange(new[]
            {
                new Product { Name = "Cotton T-Shirt", SKU = "CLTH-001", Barcode = "2234567890123", Price = 19.99m, Cost = 8.00m, Quantity = 300, MinimumStockLevel = 100, CategoryId = clothing.Id },
                new Product { Name = "Denim Jeans", SKU = "CLTH-002", Barcode = "2234567890124", Price = 49.99m, Cost = 25.00m, Quantity = 150, MinimumStockLevel = 50, CategoryId = clothing.Id },
                new Product { Name = "Hoodie", SKU = "CLTH-003", Barcode = "2234567890125", Price = 39.99m, Cost = 18.00m, Quantity = 120, MinimumStockLevel = 40, CategoryId = clothing.Id },
                new Product { Name = "Running Shoes", SKU = "CLTH-004", Barcode = "2234567890126", Price = 79.99m, Cost = 40.00m, Quantity = 80, MinimumStockLevel = 25, CategoryId = clothing.Id },
                new Product { Name = "Baseball Cap", SKU = "CLTH-005", Barcode = "2234567890127", Price = 14.99m, Cost = 6.00m, Quantity = 200, MinimumStockLevel = 60, CategoryId = clothing.Id },
                new Product { Name = "Winter Jacket", SKU = "CLTH-006", Barcode = "2234567890128", Price = 99.99m, Cost = 50.00m, Quantity = 60, MinimumStockLevel = 20, CategoryId = clothing.Id },
                new Product { Name = "Socks Pack (6)", SKU = "CLTH-007", Barcode = "2234567890129", Price = 12.99m, Cost = 5.00m, Quantity = 400, MinimumStockLevel = 100, CategoryId = clothing.Id },
                new Product { Name = "Leather Belt", SKU = "CLTH-008", Barcode = "2234567890130", Price = 24.99m, Cost = 10.00m, Quantity = 100, MinimumStockLevel = 30, CategoryId = clothing.Id },
                new Product { Name = "Backpack", SKU = "CLTH-009", Barcode = "2234567890131", Price = 34.99m, Cost = 15.00m, Quantity = 90, MinimumStockLevel = 30, CategoryId = clothing.Id },
                new Product { Name = "Sunglasses", SKU = "CLTH-010", Barcode = "2234567890132", Price = 29.99m, Cost = 12.00m, Quantity = 150, MinimumStockLevel = 50, CategoryId = clothing.Id }
            });
        }

        // Food & Beverages (10 products)
        var food = categories.FirstOrDefault(c => c.Name == "Food & Beverages");
        if (food != null)
        {
            products.AddRange(new[]
            {
                new Product { Name = "Bottled Water 500ml", SKU = "FOOD-001", Barcode = "3234567890123", Price = 1.99m, Cost = 0.50m, Quantity = 1000, MinimumStockLevel = 300, CategoryId = food.Id },
                new Product { Name = "Energy Drink", SKU = "FOOD-002", Barcode = "3234567890124", Price = 2.99m, Cost = 1.20m, Quantity = 500, MinimumStockLevel = 150, CategoryId = food.Id },
                new Product { Name = "Chocolate Bar", SKU = "FOOD-003", Barcode = "3234567890125", Price = 3.49m, Cost = 1.50m, Quantity = 600, MinimumStockLevel = 200, CategoryId = food.Id },
                new Product { Name = "Potato Chips", SKU = "FOOD-004", Barcode = "3234567890126", Price = 4.99m, Cost = 2.00m, Quantity = 400, MinimumStockLevel = 120, CategoryId = food.Id },
                new Product { Name = "Coffee Beans 500g", SKU = "FOOD-005", Barcode = "3234567890127", Price = 12.99m, Cost = 6.00m, Quantity = 200, MinimumStockLevel = 60, CategoryId = food.Id },
                new Product { Name = "Tea Bags (50)", SKU = "FOOD-006", Barcode = "3234567890128", Price = 5.99m, Cost = 2.50m, Quantity = 300, MinimumStockLevel = 100, CategoryId = food.Id },
                new Product { Name = "Cereal Box", SKU = "FOOD-007", Barcode = "3234567890129", Price = 6.99m, Cost = 3.00m, Quantity = 250, MinimumStockLevel = 80, CategoryId = food.Id },
                new Product { Name = "Canned Soup", SKU = "FOOD-008", Barcode = "3234567890130", Price = 3.99m, Cost = 1.50m, Quantity = 350, MinimumStockLevel = 100, CategoryId = food.Id },
                new Product { Name = "Pasta 500g", SKU = "FOOD-009", Barcode = "3234567890131", Price = 2.49m, Cost = 1.00m, Quantity = 500, MinimumStockLevel = 150, CategoryId = food.Id },
                new Product { Name = "Olive Oil 500ml", SKU = "FOOD-010", Barcode = "3234567890132", Price = 8.99m, Cost = 4.00m, Quantity = 180, MinimumStockLevel = 50, CategoryId = food.Id }
            });
        }

        // Home & Garden (10 products)
        var homeGarden = categories.FirstOrDefault(c => c.Name == "Home & Garden");
        if (homeGarden != null)
        {
            products.AddRange(new[]
            {
                new Product { Name = "Garden Shovel", SKU = "HOME-001", Barcode = "4234567890123", Price = 24.99m, Cost = 12.00m, Quantity = 50, MinimumStockLevel = 15, CategoryId = homeGarden.Id },
                new Product { Name = "Plant Pot 10\"", SKU = "HOME-002", Barcode = "4234567890124", Price = 9.99m, Cost = 4.00m, Quantity = 200, MinimumStockLevel = 60, CategoryId = homeGarden.Id },
                new Product { Name = "Garden Hose 50ft", SKU = "HOME-003", Barcode = "4234567890125", Price = 34.99m, Cost = 18.00m, Quantity = 80, MinimumStockLevel = 25, CategoryId = homeGarden.Id },
                new Product { Name = "LED Light Bulb", SKU = "HOME-004", Barcode = "4234567890126", Price = 6.99m, Cost = 3.00m, Quantity = 300, MinimumStockLevel = 100, CategoryId = homeGarden.Id },
                new Product { Name = "Tool Set", SKU = "HOME-005", Barcode = "4234567890127", Price = 49.99m, Cost = 25.00m, Quantity = 60, MinimumStockLevel = 20, CategoryId = homeGarden.Id },
                new Product { Name = "Paint Brush Set", SKU = "HOME-006", Barcode = "4234567890128", Price = 14.99m, Cost = 7.00m, Quantity = 100, MinimumStockLevel = 30, CategoryId = homeGarden.Id },
                new Product { Name = "Extension Cord 25ft", SKU = "HOME-007", Barcode = "4234567890129", Price = 19.99m, Cost = 9.00m, Quantity = 120, MinimumStockLevel = 40, CategoryId = homeGarden.Id },
                new Product { Name = "Storage Box Large", SKU = "HOME-008", Barcode = "4234567890130", Price = 16.99m, Cost = 8.00m, Quantity = 90, MinimumStockLevel = 30, CategoryId = homeGarden.Id },
                new Product { Name = "Garden Gloves", SKU = "HOME-009", Barcode = "4234567890131", Price = 8.99m, Cost = 4.00m, Quantity = 150, MinimumStockLevel = 50, CategoryId = homeGarden.Id },
                new Product { Name = "Watering Can", SKU = "HOME-010", Barcode = "4234567890132", Price = 12.99m, Cost = 6.00m, Quantity = 110, MinimumStockLevel = 35, CategoryId = homeGarden.Id }
            });
        }

        // Sports & Outdoors (10 products)
        var sports = categories.FirstOrDefault(c => c.Name == "Sports & Outdoors");
        if (sports != null)
        {
            products.AddRange(new[]
            {
                new Product { Name = "Basketball", SKU = "SPRT-001", Barcode = "5234567890123", Price = 29.99m, Cost = 15.00m, Quantity = 70, MinimumStockLevel = 25, CategoryId = sports.Id },
                new Product { Name = "Yoga Mat", SKU = "SPRT-002", Barcode = "5234567890124", Price = 24.99m, Cost = 12.00m, Quantity = 100, MinimumStockLevel = 30, CategoryId = sports.Id },
                new Product { Name = "Dumbbells 10lb", SKU = "SPRT-003", Barcode = "5234567890125", Price = 34.99m, Cost = 18.00m, Quantity = 60, MinimumStockLevel = 20, CategoryId = sports.Id },
                new Product { Name = "Tennis Racket", SKU = "SPRT-004", Barcode = "5234567890126", Price = 79.99m, Cost = 40.00m, Quantity = 40, MinimumStockLevel = 15, CategoryId = sports.Id },
                new Product { Name = "Camping Tent 4-Person", SKU = "SPRT-005", Barcode = "5234567890127", Price = 149.99m, Cost = 80.00m, Quantity = 25, MinimumStockLevel = 10, CategoryId = sports.Id },
                new Product { Name = "Hiking Backpack", SKU = "SPRT-006", Barcode = "5234567890128", Price = 89.99m, Cost = 45.00m, Quantity = 45, MinimumStockLevel = 15, CategoryId = sports.Id },
                new Product { Name = "Fishing Rod", SKU = "SPRT-007", Barcode = "5234567890129", Price = 59.99m, Cost = 30.00m, Quantity = 50, MinimumStockLevel = 20, CategoryId = sports.Id },
                new Product { Name = "Bicycle Helmet", SKU = "SPRT-008", Barcode = "5234567890130", Price = 39.99m, Cost = 20.00m, Quantity = 80, MinimumStockLevel = 25, CategoryId = sports.Id },
                new Product { Name = "Jump Rope", SKU = "SPRT-009", Barcode = "5234567890131", Price = 12.99m, Cost = 6.00m, Quantity = 130, MinimumStockLevel = 40, CategoryId = sports.Id },
                new Product { Name = "Resistance Bands Set", SKU = "SPRT-010", Barcode = "5234567890132", Price = 19.99m, Cost = 10.00m, Quantity = 95, MinimumStockLevel = 30, CategoryId = sports.Id }
            });
        }

        // Get all existing products once to avoid multiple queries
        var existingProducts = (await productRepository.GetAllAsync()).ToList();
        var existingSkus = existingProducts.Select(p => p.SKU).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Create all products
        var createdProducts = new List<Product>();
        foreach (var product in products)
        {
            if (existingSkus.Contains(product.SKU))
            {
                // Product already exists, find it in the existing list
                var existing = existingProducts.FirstOrDefault(p => p.SKU.Equals(product.SKU, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    createdProducts.Add(existing);
                }
            }
            else
            {
                // New product, add it
                var created = await productRepository.AddAsync(product);
                createdProducts.Add(created);
                existingSkus.Add(created.SKU); // Update the set to avoid duplicates
            }
        }

        return createdProducts;
    }

    private static async Task SeedStockMovementsAsync(
        IStockMovementRepository stockMovementRepository,
        List<Product> products)
    {
        // Check if movements already exist - if so, skip seeding
        var existingMovements = await stockMovementRepository.GetAllAsync();
        if (existingMovements.Any())
        {
            return; // Movements already seeded, skip
        }

        var random = new Random(42); // Fixed seed for reproducibility
        var users = new[] { "admin", "moderator", "cashier" };
        var reasons = new[]
        {
            "Initial stock",
            "Restock from supplier",
            "Customer return",
            "Inventory adjustment",
            "Damaged goods removal",
            "Promotional restock",
            "End of season clearance",
            "Quality control check",
            "Bulk order received",
            "Stock correction"
        };

        var notes = new[]
        {
            "Received from main supplier",
            "Verified and counted",
            "Damaged items removed",
            "Seasonal restock",
            "Quality inspection passed",
            null,
            "Bulk discount applied",
            "Inventory audit correction",
            null,
            "Customer return processed"
        };

        // Create movements for the past 90 days
        var movements = new List<StockMovement>();
        var baseDate = DateTime.UtcNow.AddDays(-90);

        foreach (var product in products)
        {
            // Create 2-5 movements per product
            var movementCount = random.Next(2, 6);
            
            for (int i = 0; i < movementCount; i++)
            {
                var daysAgo = random.Next(0, 90);
                var createdAt = baseDate.AddDays(daysAgo).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
                
                var movementType = random.Next(1, 4); // 1: Add, 2: Remove, 3: Adjustment
                var quantity = movementType switch
                {
                    1 => random.Next(10, 100), // Add: 10-100
                    2 => random.Next(5, 50),   // Remove: 5-50
                    3 => random.Next(20, 200), // Adjustment: 20-200
                    _ => random.Next(10, 50)
                };

                var user = users[random.Next(users.Length)];
                var reason = reasons[random.Next(reasons.Length)];
                var note = notes[random.Next(notes.Length)];

                // For adjustment, we need to calculate the delta
                // But since we're seeding, we'll use a simpler approach
                var movementQuantity = movementType switch
                {
                    1 => quantity, // Add
                    2 => -quantity, // Remove
                    3 => random.Next(-20, 50), // Adjustment delta
                    _ => quantity
                };

                var movement = new StockMovement
                {
                    ProductId = product.Id,
                    MovementType = (StockMovementType)movementType,
                    Quantity = movementQuantity,
                    Reason = reason,
                    Notes = note,
                    CreatedBy = user,
                    CreatedAt = createdAt
                };

                movements.Add(movement);
            }
        }

        // Add movements in batches
        foreach (var movement in movements)
        {
            await stockMovementRepository.AddAsync(movement);
        }
    }
}

