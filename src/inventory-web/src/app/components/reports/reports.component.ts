import { Component, OnInit, OnDestroy } from '@angular/core';
import { InventoryApiService, ProductDto, StockMovementDto, PagedResultDto } from '../../services/inventory-api.service';
import { TranslationService } from '../../services/translation.service';
import { LocaleService } from '../../services/locale.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-reports',
  standalone: false,
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent implements OnInit {
  stockReport: ProductDto[] = [];
  lowStockReport: ProductDto[] = [];
  movementReport: StockMovementDto[] = [];
  
  startDate: string = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
  endDate: string = new Date().toISOString().split('T')[0];
  
  activeTab: 'stock' | 'lowStock' | 'movements' = 'stock';
  
  // Pagination for stock report
  stockCurrentPage: number = 1;
  stockPageSize: number = 10;
  stockTotalCount: number = 0;
  stockTotalPages: number = 0;
  stockHasPreviousPage: boolean = false;
  stockHasNextPage: boolean = false;
  
  // Pagination for low stock report
  lowStockCurrentPage: number = 1;
  lowStockPageSize: number = 10;
  lowStockTotalCount: number = 0;
  lowStockTotalPages: number = 0;
  lowStockHasPreviousPage: boolean = false;
  lowStockHasNextPage: boolean = false;
  
  Math = Math;

  private localeSubscription?: Subscription;

  constructor(
    private apiService: InventoryApiService,
    public translate: TranslationService,
    private localeService: LocaleService
  ) {}

  ngOnInit(): void {
    this.generateStockReport();

    // Subscribe to locale changes to reload data when language changes
    this.localeSubscription = this.localeService.currentLocale$.subscribe(() => {
      // Reload current report when locale changes to update date formatting
      if (this.activeTab === 'stock') {
        this.generateStockReport();
      } else if (this.activeTab === 'lowStock') {
        this.generateLowStockReport();
      } else if (this.activeTab === 'movements') {
        this.generateMovementReport();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.localeSubscription) {
      this.localeSubscription.unsubscribe();
    }
  }

  generateStockReport(): void {
    this.apiService.getStockReport(this.stockCurrentPage, this.stockPageSize).subscribe({
      next: (result: ProductDto[] | PagedResultDto<ProductDto>) => {
        if (Array.isArray(result)) {
          this.stockReport = result;
          this.stockTotalCount = result.length;
          this.stockTotalPages = 1;
          this.stockHasPreviousPage = false;
          this.stockHasNextPage = false;
        } else {
          this.stockReport = result.items;
          this.stockTotalCount = result.totalCount;
          this.stockTotalPages = result.totalPages;
          this.stockHasPreviousPage = result.hasPreviousPage;
          this.stockHasNextPage = result.hasNextPage;
        }
        this.activeTab = 'stock';
      },
      error: (error) => {
        console.error('Error generating stock report:', error);
        const errorMsg = this.translate.translate('error.generatingStockReport') + ': ' + (error.error?.message || error.message);
        alert(errorMsg);
      }
    });
  }
  
  stockGoToPage(page: number): void {
    if (page >= 1 && page <= this.stockTotalPages) {
      this.stockCurrentPage = page;
      this.generateStockReport();
    }
  }
  
  stockChangePageSize(size: number): void {
    this.stockPageSize = size;
    this.stockCurrentPage = 1;
    this.generateStockReport();
  }
  
  stockGetPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.stockCurrentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.stockTotalPages, startPage + maxPages - 1);
    
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  generateLowStockReport(): void {
    this.apiService.getLowStockReport(this.lowStockCurrentPage, this.lowStockPageSize).subscribe({
      next: (result: ProductDto[] | PagedResultDto<ProductDto>) => {
        if (Array.isArray(result)) {
          this.lowStockReport = result;
          this.lowStockTotalCount = result.length;
          this.lowStockTotalPages = 1;
          this.lowStockHasPreviousPage = false;
          this.lowStockHasNextPage = false;
        } else {
          this.lowStockReport = result.items;
          this.lowStockTotalCount = result.totalCount;
          this.lowStockTotalPages = result.totalPages;
          this.lowStockHasPreviousPage = result.hasPreviousPage;
          this.lowStockHasNextPage = result.hasNextPage;
        }
        this.activeTab = 'lowStock';
      },
      error: (error) => {
        console.error('Error generating low stock report:', error);
        const errorMsg = this.translate.translate('error.generatingLowStockReport') + ': ' + (error.error?.message || error.message);
        alert(errorMsg);
      }
    });
  }
  
  lowStockGoToPage(page: number): void {
    if (page >= 1 && page <= this.lowStockTotalPages) {
      this.lowStockCurrentPage = page;
      this.generateLowStockReport();
    }
  }
  
  lowStockChangePageSize(size: number): void {
    this.lowStockPageSize = size;
    this.lowStockCurrentPage = 1;
    this.generateLowStockReport();
  }
  
  lowStockGetPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.lowStockCurrentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.lowStockTotalPages, startPage + maxPages - 1);
    
    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  generateMovementReport(): void {
    console.log('Generating movement report with dates:', this.startDate, this.endDate);
    this.apiService.getMovementReport(this.startDate, this.endDate).subscribe({
      next: (movements) => {
        console.log('Received movements:', movements);
        this.movementReport = movements.sort((a, b) => 
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.activeTab = 'movements';
        if (movements.length === 0) {
          console.warn('No movements found for the selected date range');
        }
      },
      error: (error) => {
        console.error('Error generating movement report:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          error: error.error
        });
        const errorMsg = this.translate.translate('error.generatingMovementReport') + ': ' + (error.error?.message || error.message || 'Unknown error');
        alert(errorMsg);
      }
    });
  }

  exportStockReport(): void {
    this.exportToCSV(this.stockReport, 'StockReport', [
      'SKU', 'Name', 'Category', 'Quantity', 'Price', 'Low Stock'
    ], (item: ProductDto) => [
      item.sku, item.name, item.categoryName || '', item.quantity.toString(), 
      item.price.toString(), item.isLowStock ? 'Yes' : 'No'
    ]);
  }

  exportLowStockReport(): void {
    this.exportToCSV(this.lowStockReport, 'LowStockReport', [
      'SKU', 'Name', 'Category', 'Quantity', 'Threshold', 'Price'
    ], (item: ProductDto) => [
      item.sku, item.name, item.categoryName || '', item.quantity.toString(),
      item.minimumStockLevel.toString(), item.price.toString()
    ]);
  }

  exportMovementReport(): void {
    this.exportToCSV(this.movementReport, 'MovementReport', [
      'Date', 'Product', 'Type', 'Quantity', 'Reason', 'Notes'
    ], (item: StockMovementDto) => [
      new Date(item.createdAt).toLocaleString(), item.productName,
      this.getMovementTypeName(item.movementType), item.quantity.toString(),
      item.reason || '', item.notes || ''
    ]);
  }

  private exportToCSV(data: any[], filename: string, headers: string[], rowMapper: (item: any) => string[]): void {
    const csvContent = [
      headers.join(','),
      ...data.map(item => rowMapper(item).map(cell => `"${cell}"`).join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `${filename}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
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

