# AAM.Inventory.Core.Domain

[![NuGet](https://img.shields.io/nuget/v/AAM.Inventory.Core.Domain.svg)](https://www.nuget.org/packages/AAM.Inventory.Core.Domain)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

The domain layer for the Inventory Suite - a clean architecture implementation with pure domain models, entities, and interfaces.

## Features

- ✅ **Pure Domain Models**: No external dependencies
- ✅ **Entity Framework Ready**: Entities designed for EF Core
- ✅ **Clean Architecture**: Follows DDD principles
- ✅ **Type-Safe Enums**: Strongly-typed enumerations
- ✅ **Repository Pattern**: Interface definitions for data access

## Installation

```bash
dotnet add package AAM.Inventory.Core.Domain
```

Or via Package Manager:
```
Install-Package AAM.Inventory.Core.Domain
```

## Core Components

### Entities

- **Product**: Product information with SKU, name, description, and pricing
- **Category**: Product categorization
- **User**: User accounts with role-based access
- **StockMovement**: Audit trail for inventory changes

### Enums

- **UserRole**: SuperAdmin, Moderator, Cashier
- **MovementType**: Add, Remove, Adjust

### Interfaces

- Repository interfaces for data access abstraction
- Domain service interfaces

## Usage Example

```csharp
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Enums;

// Create a product
var product = new Product
{
    Name = "Sample Product",
    SKU = "SKU-001",
    Description = "A sample product",
    Price = 99.99m,
    CategoryId = 1
};

// Create a user
var user = new User
{
    Username = "john.doe",
    Email = "john@example.com",
    Role = UserRole.Moderator,
    IsActive = true
};
```

## Dependencies

This package has **no external dependencies**, making it perfect for:
- Clean Architecture implementations
- Domain-Driven Design (DDD)
- Multi-platform projects
- NuGet package distribution

## Target Framework

- .NET 8.0

## Related Packages

- [AAM.Inventory.Core.Application](https://www.nuget.org/packages/AAM.Inventory.Core.Application) - Business logic and use cases

## Documentation

For complete documentation, visit the [Inventory Suite repository](https://github.com/xx7Ahmed7xx/AAM.InventorySuite).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/xx7Ahmed7xx/AAM.InventorySuite/blob/main/LICENSE) file for details.

## Contributing

Contributions are welcome! Please see the [Contributing Guide](https://github.com/xx7Ahmed7xx/AAM.InventorySuite/blob/main/CONTRIBUTING.md) for details.

