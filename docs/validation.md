# Input Validation with FluentValidation

This document explains how FluentValidation validators are used throughout the Inventory Suite API.

## How Validators Are Registered

Validators are automatically registered and applied in `src/Inventory.API/Program.cs`:

```csharp
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
        fv.AutomaticValidationEnabled = true;
        fv.ImplicitlyValidateChildProperties = true;
    });
```

**Key Configuration:**
- `RegisterValidatorsFromAssemblyContaining<CreateProductDtoValidator>()` - Automatically discovers all validators in the `AAM.Inventory.Core.Application` assembly
- `AutomaticValidationEnabled = true` - Validators run automatically before controller actions execute
- `ImplicitlyValidateChildProperties = true` - Validates nested properties in complex objects

## How Validators Are Applied

**Validators are automatically applied** - no manual validation code needed in controllers!

When a controller action receives a DTO parameter, FluentValidation:
1. Automatically finds the matching validator (e.g., `CreateProductDto` → `CreateProductDtoValidator`)
2. Validates the DTO before the action method executes
3. If validation fails, returns `400 Bad Request` with validation errors
4. If validation passes, the action method executes normally

## Validator Usage by Controller

### ProductsController

**POST `/api/products`** - Uses `CreateProductDtoValidator`
```csharp
[HttpPost]
public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, ...)
```
- Validates: Name (required, max 200 chars), SKU (required, alphanumeric), Price (> 0), etc.

**PUT `/api/products/{id}`** - Uses `UpdateProductDtoValidator`
```csharp
[HttpPut("{id}")]
public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto, ...)
```
- Validates: Same as CreateProductDto, plus ID must be > 0

### CategoriesController

**POST `/api/categories`** - Uses `CategoryDtoValidator`
```csharp
[HttpPost]
public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto dto, ...)
```
- Validates: Name (required, max 100 chars), Description (max 500 chars)

**PUT `/api/categories/{id}`** - Uses `CategoryDtoValidator`
```csharp
[HttpPut("{id}")]
public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryDto dto, ...)
```

### UsersController

**POST `/api/users`** - Uses `CreateUserDtoValidator`
```csharp
[HttpPost]
public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, ...)
```
- Validates: Username (required, 3-100 chars, alphanumeric + underscore), Email (valid format), Password (min 6 chars)

**PUT `/api/users/{id}`** - Uses `UpdateUserDtoValidator`
```csharp
[HttpPut("{id}")]
public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto, ...)
```
- Validates: Same as CreateUserDto, plus ID must be > 0, Password optional

### StockMovementsController

**POST `/api/stockmovements/add`** - Uses `StockMovementRequestDtoValidator`
```csharp
[HttpPost("add")]
public async Task<ActionResult<StockMovementDto>> AddStock([FromBody] StockMovementRequestDto dto, ...)
```
- Validates: ProductId (> 0), Quantity (> 0), Reason (max 500 chars), Notes (max 1000 chars)

**POST `/api/stockmovements/remove`** - Uses `StockMovementRequestDtoValidator`
```csharp
[HttpPost("remove")]
public async Task<ActionResult<StockMovementDto>> RemoveStock([FromBody] StockMovementRequestDto dto, ...)
```

**POST `/api/stockmovements/adjust`** - Uses `StockMovementRequestDtoValidator`
```csharp
[HttpPost("adjust")]
public async Task<ActionResult<StockMovementDto>> AdjustStock([FromBody] StockMovementRequestDto dto, ...)
```

### AuthController

**POST `/api/auth/login`** - Uses `LoginDtoValidator`
```csharp
[HttpPost("login")]
public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto, ...)
```
- Validates: Username (required), Password (required)

## Validation Error Response

When validation fails, the API automatically returns a `400 Bad Request` with a structured error response:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "Product name is required"
    ],
    "SKU": [
      "SKU is required",
      "SKU can only contain letters, numbers, hyphens, and underscores"
    ],
    "Price": [
      "Price must be greater than zero"
    ]
  }
}
```

## Example: Testing Validation

### Valid Request
```bash
POST /api/products
{
  "name": "Test Product",
  "sku": "SKU001",
  "price": 19.99,
  "cost": 10.00,
  "initialQuantity": 100,
  "minimumStockLevel": 10
}
```
✅ Returns `201 Created` with product data

### Invalid Request
```bash
POST /api/products
{
  "name": "",
  "sku": "SKU 001",  // Contains space (invalid)
  "price": -5,       // Negative price (invalid)
  "cost": 10.00,
  "initialQuantity": 100,
  "minimumStockLevel": 10
}
```
❌ Returns `400 Bad Request` with validation errors:
```json
{
  "errors": {
    "Name": ["Product name is required"],
    "SKU": ["SKU can only contain letters, numbers, hyphens, and underscores"],
    "Price": ["Price must be greater than zero"]
  }
}
```

## Validator Rules Summary

| Validator | Key Rules |
|-----------|-----------|
| `CreateProductDtoValidator` | Name (required, max 200), SKU (required, alphanumeric), Price (> 0), Cost (≥ 0) |
| `UpdateProductDtoValidator` | Same as Create, plus ID > 0 |
| `CreateUserDtoValidator` | Username (3-100 chars, alphanumeric+underscore), Email (valid format), Password (min 6 chars) |
| `UpdateUserDtoValidator` | Same as Create, plus ID > 0, Password optional |
| `CategoryDtoValidator` | Name (required, max 100), Description (max 500) |
| `StockMovementRequestDtoValidator` | ProductId (> 0), Quantity (> 0), Reason (max 500), Notes (max 1000) |
| `LoginDtoValidator` | Username (required), Password (required) |

## Benefits

1. **Automatic Validation**: No manual `if` statements or `ModelState.IsValid` checks needed
2. **Consistent Error Responses**: All validation errors follow the same format
3. **Type Safety**: Validators are strongly typed to their DTOs
4. **Reusable**: Validators can be used in other contexts (e.g., background jobs, console apps)
5. **Testable**: Validators can be unit tested independently
6. **Maintainable**: Validation rules are centralized in validator classes

## Adding New Validators

To add validation for a new DTO:

1. Create a validator class in `src/Inventory.Core.Application/Validators/`:
   ```csharp
   public class MyDtoValidator : AbstractValidator<MyDto>
   {
       public MyDtoValidator()
       {
           RuleFor(x => x.Property)
               .NotEmpty()
               .WithMessage("Property is required");
       }
   }
   ```

2. That's it! The validator is automatically discovered and applied to any controller action that uses `MyDto`.

No additional registration or controller code needed!

