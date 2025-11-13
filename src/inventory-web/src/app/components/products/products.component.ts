import { Component, OnInit } from '@angular/core';
import { InventoryApiService, ProductDto, CreateProductDto, UpdateProductDto, CategoryDto } from '../../services/inventory-api.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit {
  products: ProductDto[] = [];
  categories: CategoryDto[] = [];
  searchTerm: string = '';
  selectedProduct: ProductDto | null = null;
  showProductForm: boolean = false;
  isEditing: boolean = false;
  
  // Pagination
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;
  
  productForm: any = {
    name: '',
    sku: '',
    price: 0,
    cost: 0,
    initialQuantity: 0,
    minimumStockLevel: 0
  };

  constructor(
    private apiService: InventoryApiService,
    public authService: AuthService,
    public translate: TranslationService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts(): void {
    this.apiService.getProducts(this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        if (Array.isArray(result)) {
          this.products = result;
          this.totalCount = result.length;
          this.totalPages = 1;
          this.hasPreviousPage = false;
          this.hasNextPage = false;
        } else {
          this.products = result.items;
          this.totalCount = result.totalCount;
          this.totalPages = result.totalPages;
          this.hasPreviousPage = result.hasPreviousPage;
          this.hasNextPage = result.hasNextPage;
        }
      },
        error: (error) => {
          console.error('Error loading products:', error);
          alert(this.translate.translate('error.loadingProducts'));
        }
    });
  }
  
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProducts();
    }
  }
  
  changePageSize(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadProducts();
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

  loadCategories(): void {
    this.apiService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  search(): void {
    if (!this.searchTerm.trim()) {
      this.currentPage = 1;
      this.loadProducts();
      return;
    }

    this.apiService.searchProducts(this.searchTerm).subscribe({
      next: (products) => {
        this.products = products;
        this.totalCount = products.length;
        this.totalPages = 1;
        this.hasPreviousPage = false;
        this.hasNextPage = false;
      },
      error: (error) => {
        console.error('Error searching products:', error);
      }
    });
  }

  showLowStock(): void {
    this.apiService.getLowStockProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.totalCount = products.length;
        this.totalPages = 1;
        this.hasPreviousPage = false;
        this.hasNextPage = false;
      },
      error: (error) => {
        console.error('Error loading low stock products:', error);
      }
    });
  }

  addProduct(): void {
    this.isEditing = false;
    this.productForm = {
      name: '',
      sku: '',
      price: 0,
      cost: 0,
      initialQuantity: 0,
      minimumStockLevel: 0
    };
    this.selectedProduct = null;
    this.showProductForm = true;
  }

  editProduct(product: ProductDto): void {
    this.isEditing = true;
    this.selectedProduct = product;
    this.productForm = {
      id: product.id,
      name: product.name,
      description: product.description,
      sku: product.sku,
      barcode: product.barcode,
      price: product.price,
      cost: product.cost,
      quantity: product.quantity,
      minimumStockLevel: product.minimumStockLevel,
      categoryId: product.categoryId
    };
    this.showProductForm = true;
  }

  saveProduct(): void {
    // Ensure categoryId is null instead of undefined or 0 when "None" is selected
    const formData = { ...this.productForm };
    if (formData.categoryId === undefined || formData.categoryId === 0 || formData.categoryId === '') {
      formData.categoryId = null;
    }
    
    if (this.isEditing) {
      this.apiService.updateProduct(formData as UpdateProductDto).subscribe({
        next: () => {
          this.currentPage = 1;
          this.loadProducts();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error updating product:', error);
          const errorMsg = this.translate.translate('error.updatingProduct') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    } else {
      this.apiService.createProduct(formData as CreateProductDto).subscribe({
        next: () => {
          this.currentPage = 1;
          this.loadProducts();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error creating product:', error);
          const errorMsg = this.translate.translate('error.creatingProduct') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  deleteProduct(product: ProductDto): void {
    const confirmMessage = this.translate.translateWithParams('confirm.deleteProduct', { name: product.name });
    if (confirm(confirmMessage)) {
      this.apiService.deleteProduct(product.id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          const errorMsg = this.translate.translate('error.deletingProduct') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  cancelForm(): void {
    this.showProductForm = false;
    this.selectedProduct = null;
    this.isEditing = false;
  }
}

