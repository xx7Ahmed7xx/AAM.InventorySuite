# NuGet Packaging Guide

## Overview

The InventorySuite core libraries are designed to be published as NuGet packages, allowing other developers to use the domain models and business logic in their own projects.

## Packageable Projects

### ✅ Inventory.Core.Domain

**Purpose**: Pure domain models, entities, value objects, and repository interfaces.

**Why it's perfect for NuGet:**
- No external dependencies (or minimal, well-known ones)
- Pure business logic
- Reusable across different projects
- Framework-agnostic

**What to include:**
- Entity classes (Product, StockMovement, etc.)
- Value objects
- Domain events
- Repository interfaces (IProductRepository, etc.)
- Domain exceptions

**What NOT to include:**
- Database-specific code
- Infrastructure concerns
- UI-specific code

### ✅ Inventory.Core.Application

**Purpose**: Business logic, use cases, commands, queries, and DTOs.

**Why it's perfect for NuGet:**
- Depends only on Domain
- Contains reusable business logic
- Can be used with different infrastructure implementations

**What to include:**
- Use cases / Application services
- Commands and Queries (CQRS pattern)
- DTOs / ViewModels
- Validation logic
- Application-level exceptions

**What NOT to include:**
- Infrastructure implementations
- Database access code
- External service clients

### ⚠️ Inventory.Core.Infrastructure

**Purpose**: Data access implementations, external service clients.

**Recommendation**: 
- **Option 1 (Recommended)**: Keep internal, don't package. Let consumers implement their own infrastructure.
- **Option 2**: Package as separate packages (e.g., `Inventory.Core.Infrastructure.Sqlite`, `Inventory.Core.Infrastructure.SqlServer`)

**Why keep it internal:**
- Contains implementation details
- Different projects may need different databases
- Allows flexibility for consumers

## Packaging Configuration

### Step 1: Update .csproj Files

Add NuGet package metadata to your `.csproj` files:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- NuGet Package Metadata -->
    <PackageId>Inventory.Core.Domain</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Company>Your Company</Company>
    <Description>Domain models and interfaces for inventory management</Description>
    <PackageTags>inventory;domain;ddd</PackageTags>
    <RepositoryUrl>https://github.com/yourusername/InventorySuite</RepositoryUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
  </PropertyGroup>
</Project>
```

### Step 2: Create .nuspec Files (Optional)

For more control, you can create `.nuspec` files, but the `.csproj` approach is simpler and recommended.

### Step 3: Pack the Packages

```bash
# Pack Domain
dotnet pack src/Inventory.Core.Domain/Inventory.Core.Domain.csproj -c Release -o ./nupkg

# Pack Application
dotnet pack src/Inventory.Core.Application/Inventory.Core.Application.csproj -c Release -o ./nupkg
```

### Step 4: Test Locally

```bash
# Add local package source
dotnet nuget add source ./nupkg --name local

# Install in a test project
dotnet add package Inventory.Core.Domain --version 1.0.0 --source local
```

### Step 5: Publish to NuGet.org

```bash
# Publish to NuGet.org (requires API key)
dotnet nuget push ./nupkg/Inventory.Core.Domain.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Or use the automated GitHub Actions workflow (see `.github/workflows/release.yml`).

## Versioning Strategy

Follow [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

Example: `1.2.3`

## Package Dependencies

When packaging:

- **Inventory.Core.Domain**: Should have minimal or no dependencies
- **Inventory.Core.Application**: Should depend on `Inventory.Core.Domain` (as a NuGet package reference)

## Example: Using the Packages

After publishing, consumers can install:

```bash
dotnet add package Inventory.Core.Domain
dotnet add package Inventory.Core.Application
```

Then implement their own infrastructure:

```csharp
// Consumer's project
public class MyProductRepository : IProductRepository
{
    // Custom implementation
}
```

## Best Practices

1. **Keep packages focused**: Each package should have a single responsibility
2. **Minimize dependencies**: Fewer dependencies = easier to use
3. **Document well**: Include XML documentation comments
4. **Version carefully**: Follow semantic versioning
5. **Test packages**: Test the packages in isolation before publishing
6. **Provide examples**: Include sample code in documentation

## About Inventory.Core.Common

**Question**: Should `Inventory.Core.Common` be packaged?

**Answer**: It depends on what you put in it:

- **If it contains shared utilities** (extension methods, helpers): Yes, package it as `Inventory.Core.Common`
- **If it's empty or redundant**: Remove it and move utilities to Domain or Application
- **If it contains cross-cutting concerns**: Consider if they're truly reusable or project-specific

**Recommendation**: For a minimal open-source project, **remove it** unless you have a specific, reusable purpose for it. Common utilities can go in:
- Domain project (if domain-related)
- Application project (if application-related)
- Or create a separate `Inventory.Core.Shared` package if truly cross-cutting

