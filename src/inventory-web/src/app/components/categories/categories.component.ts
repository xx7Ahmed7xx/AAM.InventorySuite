import { Component, OnInit } from '@angular/core';
import { InventoryApiService, CategoryDto } from '../../services/inventory-api.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-categories',
  standalone: false,
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnInit {
  categories: CategoryDto[] = [];
  showCategoryForm: boolean = false;
  isEditing: boolean = false;
  selectedCategory: CategoryDto | null = null;
  
  categoryForm: any = {
    name: '',
    description: ''
  };

  constructor(
    private apiService: InventoryApiService,
    public authService: AuthService,
    public translate: TranslationService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.apiService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        alert(this.translate.translate('error.loadingCategories'));
      }
    });
  }

  addCategory(): void {
    this.isEditing = false;
    this.categoryForm = { name: '', description: '' };
    this.selectedCategory = null;
    this.showCategoryForm = true;
  }

  editCategory(category: CategoryDto): void {
    this.isEditing = true;
    this.selectedCategory = category;
    this.categoryForm = {
      id: category.id,
      name: category.name,
      description: category.description
    };
    this.showCategoryForm = true;
  }

  saveCategory(): void {
    if (this.isEditing) {
      this.apiService.updateCategory(this.categoryForm as CategoryDto).subscribe({
        next: () => {
          this.loadCategories();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error updating category:', error);
          const errorMsg = this.translate.translate('error.updatingCategory') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    } else {
      this.apiService.createCategory(this.categoryForm as CategoryDto).subscribe({
        next: () => {
          this.loadCategories();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error creating category:', error);
          const errorMsg = this.translate.translate('error.creatingCategory') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  deleteCategory(category: CategoryDto): void {
    const confirmMessage = this.translate.translateWithParams('confirm.deleteCategory', { name: category.name });
    if (confirm(confirmMessage)) {
      this.apiService.deleteCategory(category.id).subscribe({
        next: () => {
          this.loadCategories();
        },
        error: (error) => {
          console.error('Error deleting category:', error);
          const errorMsg = this.translate.translate('error.deletingCategory') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  cancelForm(): void {
    this.showCategoryForm = false;
    this.selectedCategory = null;
    this.isEditing = false;
  }
}

