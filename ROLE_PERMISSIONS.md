# Role Permissions Documentation

This document outlines the permissions and access levels for each user role in the Inventory Suite application.

## Overview

The Inventory Suite implements a three-tier role-based access control (RBAC) system:
1. **Cashier** - Basic inventory operations
2. **Moderator** - Full inventory management
3. **SuperAdmin** - Complete system administration

All roles require authentication via JWT tokens, and permissions are enforced at both the API and UI levels.

## Current Role Hierarchy

1. **Cashier** (Role ID: 1)
2. **Moderator** (Role ID: 2)
3. **SuperAdmin** (Role ID: 3)

## Role Permissions

### Cashier
- ✅ **View Products**: Can view all products, search, filter by category, view by SKU/barcode
- ✅ **View Categories**: Can view all categories
- ✅ **Stock Movements**: Can add stock, remove stock, and adjust stock quantities
- ❌ **Create/Edit/Delete Products**: **RESTRICTED** - Only Moderator and SuperAdmin can create, edit, or delete products
- ❌ **Create/Edit/Delete Categories**: **RESTRICTED** - Only Moderator and SuperAdmin can create, edit, or delete categories
- ❌ **Reports**: Cannot access reports (Moderator+ only)
- ❌ **User Management**: Cannot access user management (SuperAdmin only)

### Moderator
- ✅ **All Cashier Permissions**: Everything a Cashier can do
- ✅ **Create/Edit/Delete Products**: Can create, update, and delete products
- ✅ **Create/Edit/Delete Categories**: Can create, update, and delete categories
- ✅ **Reports**: Can view all reports (Stock Report, Low Stock Report, Movement History)
- ❌ **User Management**: Cannot access user management

### SuperAdmin
- ✅ **All Moderator Permissions**: Everything a Moderator can do
- ✅ **User Management**: Can create, read, update, and delete users
  - Can view all users
  - Can create new users with any role
  - Can update user details (username, email, password, role, active status)
  - Can delete users
- ✅ **Full System Access**: Complete administrative control

## API Authorization Summary

### Products Controller (`/api/products`)
- **Authorization**: `[Authorize]` - All authenticated users for GET operations
- **POST/PUT/DELETE**: `[Authorize(Roles = "Moderator,SuperAdmin")]` - Only Moderator and SuperAdmin can create, update, or delete products
- **Current State**: 
  - GET operations: All authenticated users
  - POST/PUT/DELETE: Moderator and SuperAdmin only

### Categories Controller (`/api/categories`)
- **Authorization**: `[Authorize]` - All authenticated users for GET operations
- **POST/PUT/DELETE**: `[Authorize(Roles = "Moderator,SuperAdmin")]` - Only Moderator and SuperAdmin can create, update, or delete categories
- **Current State**: 
  - GET operations: All authenticated users
  - POST/PUT/DELETE: Moderator and SuperAdmin only

### Stock Movements Controller (`/api/stockmovements`)
- **Authorization**: `[Authorize]` - All authenticated users
- **Current State**: All roles can add, remove, and adjust stock

### Reports Controller (`/api/reports`)
- **Authorization**: `[Authorize(Roles = "Moderator,SuperAdmin")]`
- **Access**: Only Moderator and SuperAdmin can access reports
- **Endpoints**:
  - `/api/reports/stock` - Stock report
  - `/api/reports/low-stock` - Low stock report
  - `/api/reports/movement-history` - Movement history report

### Users Controller (`/api/users`)
- **Authorization**: `[Authorize(Roles = "SuperAdmin")]`
- **Access**: Only SuperAdmin can access user management
- **Endpoints**:
  - `GET /api/users` - Get all users
  - `GET /api/users/{id}` - Get user by ID
  - `POST /api/users` - Create new user
  - `PUT /api/users/{id}` - Update user
  - `DELETE /api/users/{id}` - Delete user

## UI Visibility

### Angular Web App
- **Users Menu Item**: Only visible to SuperAdmin (`*ngIf="authService.isSuperAdmin()"`)
- **Reports Menu Item**: Only visible to Moderator and SuperAdmin (`*ngIf="authService.isModerator()"`)
- **Products/Categories Create/Edit/Delete Buttons**: Only visible to Moderator and SuperAdmin (`*ngIf="authService.isModerator()"`)
- **Products, Categories, Stock Movements Pages**: Visible to all authenticated users (read-only for Cashiers)

### WPF Desktop App
- **Users Tab & Menu Item**: Hidden for Cashier and Moderator, visible only to SuperAdmin
- **Reports Tab & Menu Item**: Hidden for Cashier, visible to Moderator and SuperAdmin
- **Products/Categories Create/Edit/Delete Buttons**: Hidden for Cashier, visible to Moderator and SuperAdmin (using `IsModerator` property)
- **Products, Categories, Stock Movements Tabs**: Visible to all authenticated users (read-only for Cashiers)

## Implementation Status

✅ **All role restrictions have been enforced:**

1. ✅ **Product Management**: POST/PUT/DELETE operations restricted to Moderator+ only (API + UI)
2. ✅ **Category Management**: POST/PUT/DELETE operations restricted to Moderator+ only (API + UI)
3. ✅ **Reports Access**: Restricted to Moderator+ only (API + UI)
4. ✅ **User Management**: Restricted to SuperAdmin only (API + UI)
5. ✅ **UI Synchronization**: Both Angular and WPF apps hide/show UI elements based on user role, matching backend restrictions

## Permission Matrix

| Feature | Cashier | Moderator | SuperAdmin |
|---------|---------|-----------|------------|
| View Products | ✅ | ✅ | ✅ |
| View Categories | ✅ | ✅ | ✅ |
| View Stock Movements | ✅ | ✅ | ✅ |
| Add/Remove/Adjust Stock | ✅ | ✅ | ✅ |
| Create/Edit/Delete Products | ❌ | ✅ | ✅ |
| Create/Edit/Delete Categories | ❌ | ✅ | ✅ |
| View Reports | ❌ | ✅ | ✅ |
| Generate Reports | ❌ | ✅ | ✅ |
| View Users | ❌ | ❌ | ✅ |
| Create/Edit/Delete Users | ❌ | ❌ | ✅ |
| Manage System Settings | ❌ | ❌ | ✅ |

## Recommendations for Future Improvements

1. **Audit Logging**: Track which role performed which operations (currently logged via Serilog)
2. **Password Policy**: Implement password complexity requirements (currently minimum 6 characters)
3. **Session Management**: Add session timeout and refresh token functionality
4. **Role-Based Filtering**: Consider adding additional filtering options based on roles (e.g., Cashiers can only see their own stock movements)
5. **Permission Granularity**: Add more granular permissions (e.g., view-only reports, edit-only products)

## Current Implementation Notes

- The API uses JWT tokens with role claims for authorization
- Role information is stored in the JWT token and validated on each request
- Frontend applications hide UI elements based on role, but API-level authorization is the source of truth
- If a user attempts to access a restricted endpoint, they will receive a 403 Forbidden response

