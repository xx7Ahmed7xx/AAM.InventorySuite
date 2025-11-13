import { Component, OnInit, OnDestroy } from '@angular/core';
import { InventoryApiService, UserDto, CreateUserDto, UpdateUserDto } from '../../services/inventory-api.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translation.service';
import { LocaleService } from '../../services/locale.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  users: UserDto[] = [];
  showUserForm: boolean = false;
  isEditing: boolean = false;
  selectedUser: UserDto | null = null;
  
  userForm: any = {
    username: '',
    email: '',
    password: '',
    role: 'Cashier',
    isActive: true
  };

  roles = ['Cashier', 'Moderator', 'SuperAdmin'];
  
  // Pagination
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;

  private localeSubscription?: Subscription;

  constructor(
    private apiService: InventoryApiService,
    private authService: AuthService,
    public translate: TranslationService,
    private localeService: LocaleService
  ) {}

  ngOnInit(): void {
    // Only load if SuperAdmin
    if (this.authService.isSuperAdmin()) {
      this.loadUsers();
    }

    // Subscribe to locale changes to reload data when language changes
    this.localeSubscription = this.localeService.currentLocale$.subscribe(() => {
      // Reload users when locale changes to update date formatting
      if (this.authService.isSuperAdmin()) {
        this.loadUsers();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.localeSubscription) {
      this.localeSubscription.unsubscribe();
    }
  }

  loadUsers(): void {
    this.apiService.getUsers(this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        if (Array.isArray(result)) {
          this.users = result;
          this.totalCount = result.length;
          this.totalPages = 1;
          this.hasPreviousPage = false;
          this.hasNextPage = false;
        } else {
          this.users = result.items;
          this.totalCount = result.totalCount;
          this.totalPages = result.totalPages;
          this.hasPreviousPage = result.hasPreviousPage;
          this.hasNextPage = result.hasNextPage;
        }
      },
      error: (error) => {
        console.error('Error loading users:', error);
        if (error.status === 403) {
          alert(this.translate.translate('error.accessDenied'));
        } else {
          alert(this.translate.translate('error.loadingUsers'));
        }
      }
    });
  }
  
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }
  
  changePageSize(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadUsers();
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

  addUser(): void {
    this.isEditing = false;
    this.userForm = { username: '', email: '', password: '', role: 'Cashier', isActive: true };
    this.selectedUser = null;
    this.showUserForm = true;
  }

  editUser(user: UserDto): void {
    this.isEditing = true;
    this.selectedUser = user;
    this.userForm = {
      id: user.id,
      username: user.username,
      email: user.email,
      password: '', // Don't pre-fill password
      role: user.role,
      isActive: user.isActive
    };
    this.showUserForm = true;
  }

  saveUser(): void {
    if (!this.userForm.username || !this.userForm.email) {
      alert(this.translate.translate('error.usernameEmailRequired'));
      return;
    }

    if (!this.isEditing && !this.userForm.password) {
      alert(this.translate.translate('error.passwordRequired'));
      return;
    }

    if (this.isEditing && this.selectedUser) {
      const updateDto: UpdateUserDto = {
        id: this.selectedUser.id,
        username: this.userForm.username,
        email: this.userForm.email,
        password: undefined, // Don't allow password changes when editing - field is hidden
        role: this.userForm.role,
        isActive: this.userForm.isActive
      };

      this.apiService.updateUser(updateDto).subscribe({
        next: () => {
          this.currentPage = 1;
          this.loadUsers();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error updating user:', error);
          const errorMsg = this.translate.translate('error.updatingUser') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    } else {
      const createDto: CreateUserDto = {
        username: this.userForm.username,
        email: this.userForm.email,
        password: this.userForm.password,
        role: this.userForm.role,
        isActive: this.userForm.isActive
      };

      this.apiService.createUser(createDto).subscribe({
        next: () => {
          this.currentPage = 1;
          this.loadUsers();
          this.cancelForm();
        },
        error: (error) => {
          console.error('Error creating user:', error);
          const errorMsg = this.translate.translate('error.creatingUser') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  deleteUser(user: UserDto): void {
    const confirmMessage = this.translate.translateWithParams('confirm.deleteUser', { username: user.username });
    if (confirm(confirmMessage)) {
      this.apiService.deleteUser(user.id).subscribe({
        next: () => {
          this.loadUsers();
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          const errorMsg = this.translate.translate('error.deletingUser') + ': ' + (error.error?.message || error.message);
          alert(errorMsg);
        }
      });
    }
  }

  cancelForm(): void {
    this.showUserForm = false;
    this.isEditing = false;
    this.selectedUser = null;
    this.userForm = { username: '', email: '', password: '', role: 'Cashier', isActive: true };
  }

  isSuperAdmin(): boolean {
    return this.authService.isSuperAdmin();
  }
}

