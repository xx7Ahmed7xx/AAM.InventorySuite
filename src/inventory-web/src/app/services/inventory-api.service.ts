import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProductDto {
  id: number;
  name: string;
  description?: string;
  sku: string;
  barcode?: string;
  price: number;
  cost: number;
  quantity: number;
  minimumStockLevel: number;
  categoryId?: number;
  categoryName?: string;
  isLowStock: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductDto {
  name: string;
  description?: string;
  sku: string;
  barcode?: string;
  price: number;
  cost: number;
  initialQuantity: number;
  minimumStockLevel: number;
  categoryId?: number;
}

export interface UpdateProductDto {
  id: number;
  name: string;
  description?: string;
  sku: string;
  barcode?: string;
  price: number;
  cost: number;
  quantity?: number;
  minimumStockLevel: number;
  categoryId?: number;
}

export interface CategoryDto {
  id: number;
  name: string;
  description?: string;
  productCount: number;
  createdAt: string;
}

export interface StockMovementDto {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  movementType: number;
  movementTypeName: string;
  quantity: number;
  reason?: string;
  notes?: string;
  createdBy?: string;
  createdAt: string;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryApiService {
  private apiUrl = 'https://localhost:7210/api';

  constructor(private http: HttpClient) {}

  // Products
  getProducts(pageNumber?: number, pageSize?: number): Observable<ProductDto[] | PagedResultDto<ProductDto>> {
    if (pageNumber && pageSize) {
      return this.http.get<PagedResultDto<ProductDto>>(`${this.apiUrl}/products?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    }
    return this.http.get<ProductDto[]>(`${this.apiUrl}/products`);
  }

  getProduct(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/products/${id}`);
  }

  getProductBySku(sku: string): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/products/sku/${sku}`);
  }

  searchProducts(term: string): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(`${this.apiUrl}/products/search?term=${encodeURIComponent(term)}`);
  }

  getLowStockProducts(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(`${this.apiUrl}/products/low-stock`);
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(`${this.apiUrl}/products`, product);
  }

  updateProduct(product: UpdateProductDto): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.apiUrl}/products/${product.id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${id}`);
  }

  // Categories
  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${this.apiUrl}/categories`);
  }

  getCategory(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/categories/${id}`);
  }

  createCategory(category: CategoryDto): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(`${this.apiUrl}/categories`, category);
  }

  updateCategory(category: CategoryDto): Observable<CategoryDto> {
    return this.http.put<CategoryDto>(`${this.apiUrl}/categories/${category.id}`, category);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/categories/${id}`);
  }

  // Stock Movements
  getStockMovements(pageNumber?: number, pageSize?: number): Observable<StockMovementDto[] | PagedResultDto<StockMovementDto>> {
    if (pageNumber && pageSize) {
      return this.http.get<PagedResultDto<StockMovementDto>>(`${this.apiUrl}/stockmovements?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    }
    return this.http.get<StockMovementDto[]>(`${this.apiUrl}/stockmovements`);
  }

  getStockMovementsByProduct(productId: number): Observable<StockMovementDto[]> {
    return this.http.get<StockMovementDto[]>(`${this.apiUrl}/stockmovements/product/${productId}`);
  }

  addStock(movement: any): Observable<StockMovementDto> {
    return this.http.post<StockMovementDto>(`${this.apiUrl}/stockmovements/add`, movement);
  }

  removeStock(movement: any): Observable<StockMovementDto> {
    return this.http.post<StockMovementDto>(`${this.apiUrl}/stockmovements/remove`, movement);
  }

  adjustStock(movement: any): Observable<StockMovementDto> {
    return this.http.post<StockMovementDto>(`${this.apiUrl}/stockmovements/adjust`, movement);
  }

  // Reports
  getStockReport(pageNumber?: number, pageSize?: number): Observable<ProductDto[] | PagedResultDto<ProductDto>> {
    if (pageNumber && pageSize) {
      return this.http.get<PagedResultDto<ProductDto>>(`${this.apiUrl}/reports/stock?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    }
    return this.http.get<ProductDto[]>(`${this.apiUrl}/reports/stock`);
  }

  getLowStockReport(pageNumber?: number, pageSize?: number): Observable<ProductDto[] | PagedResultDto<ProductDto>> {
    if (pageNumber && pageSize) {
      return this.http.get<PagedResultDto<ProductDto>>(`${this.apiUrl}/reports/low-stock?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    }
    return this.http.get<ProductDto[]>(`${this.apiUrl}/reports/low-stock`);
  }

  getMovementReport(startDate?: string, endDate?: string): Observable<StockMovementDto[]> {
    let url = `${this.apiUrl}/reports/movements`;
    const params: string[] = [];
    if (startDate) params.push(`startDate=${encodeURIComponent(startDate)}`);
    if (endDate) params.push(`endDate=${encodeURIComponent(endDate)}`);
    if (params.length > 0) url += '?' + params.join('&');
    return this.http.get<StockMovementDto[]>(url);
  }

  // Users (SuperAdmin only)
  getUsers(pageNumber?: number, pageSize?: number): Observable<UserDto[] | PagedResultDto<UserDto>> {
    if (pageNumber && pageSize) {
      return this.http.get<PagedResultDto<UserDto>>(`${this.apiUrl}/users?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    }
    return this.http.get<UserDto[]>(`${this.apiUrl}/users`);
  }

  getUser(id: number): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/users/${id}`);
  }

  createUser(user: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/users`, user);
  }

  updateUser(user: UpdateUserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/users/${user.id}`, user);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${id}`);
  }
}

export interface UserDto {
  id: number;
  username: string;
  email: string;
  role: string;
  isActive: boolean;
  lastLoginDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  role: string;
  isActive: boolean;
}

export interface UpdateUserDto {
  id: number;
  username: string;
  email: string;
  password?: string;
  role: string;
  isActive: boolean;
}

