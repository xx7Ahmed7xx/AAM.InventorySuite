import { Component, OnInit } from '@angular/core';
import { InventoryApiService, StockMovementDto, ProductDto } from '../../services/inventory-api.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-stock-movements',
  standalone: false,
  templateUrl: './stock-movements.component.html',
  styleUrl: './stock-movements.component.css'
})
export class StockMovementsComponent implements OnInit {
  movements: StockMovementDto[] = [];
  products: ProductDto[] = [];
  selectedProduct: ProductDto | null = null;
  movementType: number = 1; // 1: Add, 2: Remove, 3: Adjustment
  quantity: number = 1;
  reason: string = '';
  notes: string = '';
  
  // Pagination
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;

  constructor(
    private apiService: InventoryApiService,
    private authService: AuthService,
    public translate: TranslationService
  ) {}

  ngOnInit(): void {
    this.loadMovements();
    this.loadProducts();
  }

  loadMovements(): void {
    this.apiService.getStockMovements(this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        if (Array.isArray(result)) {
          this.movements = result.sort((a, b) => 
            new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
          );
          this.totalCount = result.length;
          this.totalPages = 1;
          this.hasPreviousPage = false;
          this.hasNextPage = false;
        } else {
          this.movements = result.items;
          this.totalCount = result.totalCount;
          this.totalPages = result.totalPages;
          this.hasPreviousPage = result.hasPreviousPage;
          this.hasNextPage = result.hasNextPage;
        }
      },
      error: (error) => {
        console.error('Error loading movements:', error);
        alert(this.translate.translate('error.loadingMovements'));
      }
    });
  }
  
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadMovements();
    }
  }
  
  changePageSize(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadMovements();
  }
  
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);
    
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }
  
  Math = Math;

  loadProducts(): void {
    // Load all products for the dropdown (no pagination needed)
    // Request a large page size to get all products
    this.apiService.getProducts(1, 1000).subscribe({
      next: (result) => {
        if (Array.isArray(result)) {
          this.products = result;
        } else {
          this.products = result.items;
          // If there are more products, we might need to load additional pages
          // For now, 1000 should be enough for most use cases
        }
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });
  }

  executeMovement(): void {
    if (!this.selectedProduct || this.quantity <= 0) {
      alert(this.translate.translate('error.selectProductQuantity'));
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    const movement = {
      productId: this.selectedProduct.id,
      movementType: this.movementType,
      quantity: this.quantity,
      reason: this.reason,
      notes: this.notes,
      createdBy: currentUser?.username || currentUser?.email || 'Unknown'
    };

    const operation = this.movementType === 1 
      ? this.apiService.addStock(movement)
      : this.movementType === 2
      ? this.apiService.removeStock(movement)
      : this.apiService.adjustStock(movement);

    operation.subscribe({
      next: () => {
        alert(this.translate.translate('error.movementSuccess'));
        this.quantity = 1;
        this.reason = '';
        this.notes = '';
        this.currentPage = 1;
        this.loadMovements();
        this.loadProducts();
      },
      error: (error) => {
        console.error('Error executing movement:', error);
        const errorMsg = this.translate.translate('error.executingMovement') + ': ' + (error.error?.message || error.message);
        alert(errorMsg);
      }
    });
  }

  getMovementTypeName(type: number): string {
    switch (type) {
      case 1: return 'Add';
      case 2: return 'Remove';
      case 3: return 'Adjustment';
      default: return 'Unknown';
    }
  }
}

