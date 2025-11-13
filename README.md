# InventorySuite

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18+-DD0031?logo=angular)](https://angular.io/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Web-lightgrey)]()

A modern, cross-platform inventory management system built with .NET 8, featuring both a WPF desktop application and an Angular web interface. The core business logic is designed to be reusable as NuGet packages.

## ✨ Key Features

- **📦 Complete Inventory Management**: Track products, categories, and stock movements
- **🔐 Role-Based Access Control**: SuperAdmin, Moderator, and Cashier roles with granular permissions
- **🌐 Dual UI**: Native WPF desktop app and modern Angular web interface
- **📊 Comprehensive Reporting**: Stock reports, low-stock alerts, and movement history
- **🔒 Secure Authentication**: JWT-based authentication with secure password hashing
- **✅ Input Validation**: FluentValidation for robust data validation
- **📝 Structured Logging**: Serilog for comprehensive application logging
- **🧪 Test Coverage**: Unit tests for core business logic
- **🏗️ Clean Architecture**: Domain-driven design with separation of concerns

## 🚀 Features

- **Stock Tracking**: Complete inventory management with add, remove, and adjustment operations
- **Product & Category Management**: Full CRUD operations for products and categories
- **Stock Movement History**: Complete audit trail of all stock changes
- **Reporting**: Generate CSV reports for stock levels, low-stock alerts, and movement history
- **User Authentication**: JWT-based authentication with role-based authorization (SuperAdmin, Moderator, User)
- **Dual UI**: 
  - **WPF Desktop App**: Native Windows experience with MVVM architecture
  - **Angular Web App**: Cross-platform web interface with responsive design
- **Clean Architecture**: Domain-driven design with separation of concerns
- **Extensible**: Core libraries available as NuGet packages
- **RESTful API**: ASP.NET Core Web API with Swagger documentation

## 📋 Requirements

- .NET 8.0 SDK
- Node.js 18+ (for Angular frontend)
- Windows 10+ (for WPF desktop app)
- SQLite or SQL Server LocalDB

## 🏗️ Architecture

```
InventorySuite/
├── src/
│   ├── Inventory.Core.Domain/          # Domain models, entities, interfaces
│   ├── Inventory.Core.Application/     # Business logic, use cases
│   ├── Inventory.Core.Infrastructure/  # Data access, external services
│   ├── Inventory.API/                   # ASP.NET Core Web API
│   ├── Inventory.Desktop/               # WPF desktop application
│   └── inventory-web/                   # Angular web frontend
├── tests/
│   └── Inventory.Core.Tests/            # Unit tests
└── docs/                                # Documentation
```

### Project Structure

- **Inventory.Core.Domain**: Pure domain models with no external dependencies. Perfect for NuGet packaging.
- **Inventory.Core.Application**: Business logic and use cases. Depends only on Domain.
- **Inventory.Core.Infrastructure**: Data access implementations (Entity Framework, repositories).
- **Inventory.API**: RESTful API backend for the Angular frontend.
- **Inventory.Desktop**: WPF desktop application.
- **inventory-web**: Angular single-page application.

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js](https://nodejs.org/) 18+ (for Angular frontend)
- Windows 10+ (for WPF desktop app)
- SQLite (included) or SQL Server LocalDB

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AAM.InventorySuite.git
   cd AAM.InventorySuite
   ```

2. **Configure the API**
   ```bash
   cd src/Inventory.API
   # Copy appsettings.Example.json to appsettings.json and update JWT key
   cp appsettings.Example.json appsettings.json
   # Edit appsettings.json and set a secure JWT key (at least 32 characters)
   ```

3. **Build the solution**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the API**
   ```bash
   cd src/Inventory.API
   dotnet run
   ```
   The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

5. **Run the Angular frontend** (in a new terminal)
   ```bash
   cd src/inventory-web
   npm install
   npm start
   ```
   The Angular app will be available at `http://localhost:4200`.

6. **Run the WPF desktop app** (in a new terminal)
   ```bash
   cd src/Inventory.Desktop
   dotnet run
   ```

### Default Credentials

- **Username**: `admin`
- **Password**: `admin123`
- **Role**: `SuperAdmin`

⚠️ **Important**: Change the default password immediately in production!

### Running the Desktop Application

```bash
cd src/Inventory.Desktop
dotnet run
```

### Running the API

```bash
cd src/Inventory.API
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

### Running the Angular Frontend

```bash
cd src/inventory-web
npm install
npm start
```

The Angular app will be available at `http://localhost:4200`.

## 📦 NuGet Packages

The core libraries are designed to be published as NuGet packages:

- **Inventory.Core.Domain**: Domain models and interfaces
- **Inventory.Core.Application**: Business logic and use cases

To pack the libraries:

```bash
dotnet pack src/Inventory.Core.Domain/Inventory.Core.Domain.csproj -c Release
dotnet pack src/Inventory.Core.Application/Inventory.Core.Application.csproj -c Release
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests for a specific project
dotnet test tests/Inventory.Core.Tests/
```

The test suite includes unit tests for:
- Product Service
- Category Service
- Stock Service
- Authentication Service

## 📚 Documentation

- [API Documentation](docs/api.md) - Complete API reference
- [Setup Guide](docs/setup.md) - Detailed setup instructions
- [Role Permissions](ROLE_PERMISSIONS.md) - User roles and permissions
- [Security Policy](SECURITY.md) - Security best practices and vulnerability reporting
- [NuGet Packaging](docs/nuget-packaging.md) - How to package and publish core libraries

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [.NET 8](https://dotnet.microsoft.com/)
- UI frameworks: [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/) and [Angular](https://angular.io/)
- Database: SQLite / SQL Server LocalDB

## 🔐 Default Credentials

When you first run the application, use these default credentials:
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: `SuperAdmin`

**⚠️ Important**: Change the default password in production!

## 🏗️ Architecture

The solution follows Clean Architecture principles:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌──────────────────┐      ┌──────────────────┐         │
│  │  Inventory.API   │      │ Inventory.Desktop│         │
│  │  (ASP.NET Core)  │      │     (WPF)        │         │
│  └──────────────────┘      └──────────────────┘         │
│                                                           │
│  ┌──────────────────┐                                     │
│  │  inventory-web   │                                    │
│  │    (Angular)      │                                    │
│  └──────────────────┘                                     │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                       │
│  ┌──────────────────────────────────────────────┐        │
│  │  Inventory.Core.Application                 │        │
│  │  - Services (Product, Category, Stock, etc.) │        │
│  │  - DTOs                                      │        │
│  │  - Validators (FluentValidation)             │        │
│  └──────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│                    Domain Layer                          │
│  ┌──────────────────────────────────────────────┐        │
│  │  Inventory.Core.Domain                      │        │
│  │  - Entities (Product, Category, User, etc.) │        │
│  │  - Interfaces (Repositories)                │        │
│  │  - Enums                                     │        │
│  └──────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                      │
│  ┌──────────────────────────────────────────────┐        │
│  │  Inventory.Core.Infrastructure               │        │
│  │  - Entity Framework Core                    │        │
│  │  - Repository Implementations                │        │
│  │  - Database Context                         │        │
│  └──────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────┘
```

## 📊 Project Status

This project is **production-ready** with all core features implemented:

✅ **Core Features**
- Product and Category Management
- Stock Movement Tracking
- User Authentication & Authorization
- Role-Based Access Control
- Reporting (Stock, Low Stock, Movement History)

✅ **Technical Features**
- Input Validation (FluentValidation)
- Structured Logging (Serilog)
- Unit Tests
- Clean Architecture
- JWT Authentication
- Multi-UI Support (WPF + Angular)

## 📧 Contact

For questions or support, please open an issue on GitHub.

---

**Note**: This is an open-source project. Feel free to use it as a base for your own inventory management solutions!

