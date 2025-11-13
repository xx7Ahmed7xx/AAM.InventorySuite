# Setup Guide

## Initial Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/InventorySuite.git
cd InventorySuite
```

### 2. Restore Dependencies

```bash
# Restore .NET packages
dotnet restore

# Restore Node.js packages (for Angular)
cd src/inventory-web
npm install
cd ../..
```

### 3. Database Setup

The application uses SQLite by default. The database will be created automatically on first run.

For SQL Server LocalDB:

1. Update `appsettings.json` in `Inventory.API` with your connection string
2. Run migrations (when implemented)

### 4. Build the Solution

```bash
dotnet build
```

## Running the Applications

### Desktop Application (WPF)

```bash
cd src/Inventory.Desktop
dotnet run
```

### Web API

```bash
cd src/Inventory.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Angular Web Application

```bash
cd src/inventory-web
npm start
```

The web app will be available at `http://localhost:4200`

## Development

### Running Tests

```bash
dotnet test
```

### Code Style

This project uses `.editorconfig` for consistent code style. Most IDEs will automatically apply these settings.

## Troubleshooting

### Common Issues

1. **Build fails**: Ensure you have .NET 8.0 SDK installed
2. **Angular build fails**: Run `npm install` in the `inventory-web` directory
3. **Database errors**: Check connection strings in `appsettings.json`

