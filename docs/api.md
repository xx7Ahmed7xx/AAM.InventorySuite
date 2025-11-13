# API Documentation

## Overview

The InventorySuite API is a RESTful API built with ASP.NET Core 8.0.

## Base URL

- Development: `https://localhost:5001` or `http://localhost:5000`
- Production: (configure in appsettings.json)

## Authentication

(To be implemented)

## Endpoints

### Products

#### Get All Products
```
GET /api/products
```

#### Get Product by ID
```
GET /api/products/{id}
```

#### Create Product
```
POST /api/products
```

#### Update Product
```
PUT /api/products/{id}
```

#### Delete Product
```
DELETE /api/products/{id}
```

### Stock Movements

#### Get Stock Movements
```
GET /api/stockmovements
```

#### Record Stock Movement
```
POST /api/stockmovements
```

### Reports

#### Generate Stock Report
```
GET /api/reports/stock
```

#### Generate Low Stock Alert
```
GET /api/reports/lowstock
```

#### Generate Movement History
```
GET /api/reports/movements
```

## Swagger Documentation

When running in Development mode, Swagger UI is available at:
- `https://localhost:5001/swagger`

## Response Format

All API responses follow a standard format:

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

Error responses:

```json
{
  "success": false,
  "errors": [ ... ],
  "message": "Error description"
}
```

