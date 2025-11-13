# Contributing to InventorySuite

Thank you for your interest in contributing to InventorySuite! This document provides guidelines and instructions for contributing.

## Code of Conduct

By participating in this project, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the issue
- **Expected behavior** vs **actual behavior**
- **Environment details** (OS, .NET version, etc.)
- **Screenshots** (if applicable)
- **Error messages** or logs

Use the [bug report template](.github/ISSUE_TEMPLATE/bug_report.md) when creating an issue.

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, include:

- **Clear title and description**
- **Use case** - why is this enhancement useful?
- **Proposed solution** (if you have one)
- **Alternatives considered**

Use the [feature request template](.github/ISSUE_TEMPLATE/feature_request.md) when creating an issue.

### Pull Requests

1. **Fork the repository** and create your branch from `main`
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Make your changes**
   - Follow the coding standards (see below)
   - Add tests for new functionality
   - Update documentation as needed

3. **Commit your changes**
   ```bash
   git commit -m "Add amazing feature"
   ```
   - Use clear, descriptive commit messages
   - Reference issues in commit messages (e.g., "Fix #123")

4. **Push to your fork**
   ```bash
   git push origin feature/amazing-feature
   ```

5. **Open a Pull Request**
   - Use the [PR template](.github/PULL_REQUEST_TEMPLATE.md)
   - Link to related issues
   - Ensure all CI checks pass

## Development Setup

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+ (for Angular frontend)
- Git
- Visual Studio 2022, VS Code, or Rider (recommended)

### Getting Started

1. **Clone your fork**
   ```bash
   git clone https://github.com/yourusername/AAM.InventorySuite.git
   cd AAM.InventorySuite
   ```

2. **Set up the API**
   ```bash
   cd src/Inventory.API
   cp appsettings.Example.json appsettings.json
   # Edit appsettings.json and set a secure JWT key
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   cd ../inventory-web
   npm install
   ```

4. **Build the solution**
   ```bash
   cd ../..
   dotnet build
   ```

5. **Run tests**
   ```bash
   dotnet test
   ```

## Coding Standards

### C# Code Style

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and small
- Use async/await for I/O operations

### Angular/TypeScript Code Style

- Follow [Angular Style Guide](https://angular.io/guide/styleguide)
- Use meaningful component and service names
- Add JSDoc comments for public methods
- Keep components focused and small

### Architecture Guidelines

- Follow Clean Architecture principles
- Domain layer should have no external dependencies
- Application layer depends only on Domain
- Infrastructure implements Domain interfaces
- Presentation depends on Application and Infrastructure

### Testing

- Write unit tests for business logic
- Aim for >80% code coverage on core services
- Use descriptive test names: `MethodName_Scenario_ExpectedBehavior`
- Mock external dependencies

### Commit Messages

Use clear, descriptive commit messages:

```
Add product search functionality

- Implement search by name and SKU
- Add pagination support
- Include unit tests

Fixes #123
```

## Project Structure

```
InventorySuite/
├── src/
│   ├── Inventory.Core.Domain/          # Domain models, entities, interfaces
│   ├── Inventory.Core.Application/     # Business logic, use cases
│   ├── Inventory.Core.Infrastructure/  # Data access, external services
│   ├── Inventory.API/                  # ASP.NET Core Web API
│   ├── Inventory.Desktop/              # WPF desktop application
│   └── inventory-web/                   # Angular web frontend
├── tests/
│   └── Inventory.Core.Tests/           # Unit tests
└── docs/                                # Documentation
```

## Pull Request Process

1. **Update documentation** if you've changed APIs or added features
2. **Add tests** for new functionality
3. **Ensure all tests pass** locally
4. **Update CHANGELOG.md** with your changes
5. **Request review** from maintainers

### PR Review Checklist

- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] All tests pass
- [ ] No new warnings introduced
- [ ] CHANGELOG.md updated

## Questions?

If you have questions, feel free to:
- Open a [discussion](https://github.com/yourusername/AAM.InventorySuite/discussions)
- Create an issue with the `question` label
- Contact maintainers

## Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes
- Project documentation

Thank you for contributing to InventorySuite! 🎉

