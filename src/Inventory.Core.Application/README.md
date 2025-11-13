# AAM.Inventory.Core.Application

[![NuGet](https://img.shields.io/nuget/v/AAM.Inventory.Core.Application.svg)](https://www.nuget.org/packages/AAM.Inventory.Core.Application)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

The application layer for the Inventory Suite - business logic, use cases, DTOs, and validators built on top of the domain layer.

## Features

- ✅ **Business Logic**: Application services for core operations
- ✅ **DTOs**: Data transfer objects for API communication
- ✅ **FluentValidation**: Comprehensive input validation
- ✅ **JWT Support**: Token generation and validation
- ✅ **Clean Architecture**: Depends only on Domain layer

## Installation

```bash
dotnet add package AAM.Inventory.Core.Application
```

Or via Package Manager:
```
Install-Package AAM.Inventory.Core.Application
```

## Core Components

### Services

- **ProductService**: Product management operations
- **CategoryService**: Category management
- **StockService**: Stock movement and tracking
- **UserService**: User management
- **AuthService**: Authentication and authorization

### DTOs

- **CreateProductDto**, **UpdateProductDto**: Product data transfer
- **CreateUserDto**, **UpdateUserDto**: User data transfer
- **StockMovementRequestDto**: Stock operation requests
- **LoginDto**, **AuthResponseDto**: Authentication data

### Validators

- FluentValidation validators for all DTOs
- Comprehensive validation rules
- Custom validation messages

## Usage Example

```csharp
using AAM.Inventory.Core.Application.Services;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Domain.Repositories;

// Inject the service (requires repository implementation)
var productService = new ProductService(productRepository, logger);

// Create a product
var createDto = new CreateProductDto
{
    Name = "New Product",
    SKU = "SKU-002",
    Description = "Product description",
    Price = 149.99m,
    CategoryId = 1
};

var product = await productService.CreateAsync(createDto);
```

## Dependencies

- **AAM.Inventory.Core.Domain** (required)
- **FluentValidation** (11.9.0+)
- **Microsoft.Extensions.Configuration** (8.0.0+)
- **Microsoft.IdentityModel.Tokens** (8.14.0+)
- **System.IdentityModel.Tokens.Jwt** (8.14.0+)

## Target Framework

- .NET 8.0

## Related Packages

- [AAM.Inventory.Core.Domain](https://www.nuget.org/packages/AAM.Inventory.Core.Domain) - Domain models and entities (required dependency)

## Documentation

For complete documentation, visit the [Inventory Suite repository](https://github.com/xx7Ahmed7xx/AAM.InventorySuite).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/xx7Ahmed7xx/AAM.InventorySuite/blob/main/LICENSE) file for details.

## Contributing

Contributions are welcome! Please see the [Contributing Guide](https://github.com/xx7Ahmed7xx/AAM.InventorySuite/blob/main/CONTRIBUTING.md) for details.

