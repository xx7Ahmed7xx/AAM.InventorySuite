import { Injectable } from '@angular/core';
import { LocaleService, SupportedLocale } from './locale.service';

export interface Translations {
  [key: string]: string;
}

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private translations: { [locale in SupportedLocale]: Translations } = {
    en: {
      // Navigation
      'nav.products': 'Products',
      'nav.categories': 'Categories',
      'nav.stockMovements': 'Stock Movements',
      'nav.reports': 'Reports',
      'nav.users': 'Users',
      'nav.logout': 'Logout',
      
      // Products
      'products.title': 'Products',
      'products.add': 'Add Product',
      'products.edit': 'Edit Product',
      'products.delete': 'Delete',
      'products.search': 'Search',
      'products.sku': 'SKU',
      'products.name': 'Name',
      'products.description': 'Description',
      'products.category': 'Category',
      'products.quantity': 'Quantity',
      'products.price': 'Price',
      'products.cost': 'Cost',
      'products.initialQuantity': 'Initial Quantity',
      'products.minStockLevel': 'Min Stock Level',
      'products.barcode': 'Barcode',
      'products.lowStock': 'Low Stock',
      'products.actions': 'Actions',
      'products.noProducts': 'No products found',
      'products.none': 'None',
      
      // Categories
      'categories.title': 'Categories',
      'categories.add': 'Add Category',
      'categories.edit': 'Edit Category',
      'categories.delete': 'Delete',
      'categories.name': 'Name',
      'categories.description': 'Description',
      'categories.productCount': 'Product Count',
      'categories.actions': 'Actions',
      'categories.noCategories': 'No categories found',
      
      // Stock Movements
      'stockMovements.title': 'Stock Movements',
      'stockMovements.execute': 'Execute Movement',
      'stockMovements.product': 'Product',
      'stockMovements.type': 'Type',
      'stockMovements.quantity': 'Quantity',
      'stockMovements.reason': 'Reason',
      'stockMovements.notes': 'Notes',
      'stockMovements.history': 'Movement History',
      'stockMovements.date': 'Date',
      'stockMovements.createdBy': 'Created By',
      'stockMovements.noMovements': 'No movements found',
      'stockMovements.add': 'Add Stock (increase by quantity)',
      'stockMovements.remove': 'Remove Stock (decrease by quantity)',
      'stockMovements.adjustment': 'Adjustment (set to absolute quantity)',
      'stockMovements.movementTypes': 'Add: increases stock by quantity | Remove: decreases stock by quantity | Adjustment: sets stock to absolute quantity',
      'stockMovements.selectProduct': 'Select a product...',
      
      // Reports
      'reports.title': 'Reports',
      'reports.currentStock': 'Current Stock',
      'reports.lowStock': 'Low Stock Alerts',
      'reports.movementHistory': 'Movement History',
      'reports.export': 'Export to CSV',
      'reports.startDate': 'Start Date',
      'reports.endDate': 'End Date',
      'reports.generate': 'Generate Report',
      
      // Users
      'users.title': 'User Management (SuperAdmin Only)',
      'users.add': 'Add User',
      'users.edit': 'Edit User',
      'users.delete': 'Delete',
      'users.username': 'Username',
      'users.email': 'Email',
      'users.password': 'Password',
      'users.role': 'Role',
      'users.active': 'Active',
      'users.lastLogin': 'Last Login',
      'users.actions': 'Actions',
      'users.never': 'Never',
      'users.yes': 'Yes',
      'users.no': 'No',
      
      // Common
      'common.save': 'Save',
      'common.cancel': 'Cancel',
      'common.delete': 'Delete',
      'common.edit': 'Edit',
      'common.search': 'Search',
      'common.refresh': 'Refresh',
      'common.loading': 'Loading...',
      'common.error': 'Error',
      'common.success': 'Success',
      'common.confirm': 'Confirm',
      'common.yes': 'Yes',
      'common.no': 'No',
      
      // Login
      'login.title': 'Login',
      'login.appTitle': 'Inventory Suite',
      'login.username': 'Username',
      'login.password': 'Password',
      'login.loggingIn': 'Logging in...',
      'login.defaultCredentials': 'Default credentials:',
      'login.defaultUsername': 'Username: admin',
      'login.defaultPassword': 'Password: admin123',
      'login.pleaseEnter': 'Please enter username and password',
      'login.invalid': 'Invalid username or password',
      
      // Logout
      'logout.confirm': 'Are you sure you want to logout?',
      
      // Confirmations
      'confirm.deleteProduct': 'Are you sure you want to delete "{{name}}"?',
      'confirm.deleteCategory': 'Are you sure you want to delete "{{name}}"? This will remove the category from all products.',
      'confirm.deleteUser': 'Are you sure you want to delete user \'{{username}}\'?',
      
      // Error messages
      'error.loadingProducts': 'Error loading products. Make sure the API is running.',
      'error.loadingCategories': 'Error loading categories. Make sure the API is running.',
      'error.loadingMovements': 'Error loading movements. Make sure the API is running.',
      'error.loadingUsers': 'Error loading users. Make sure the API is running.',
      'error.creatingProduct': 'Error creating product',
      'error.updatingProduct': 'Error updating product',
      'error.deletingProduct': 'Error deleting product',
      'error.creatingCategory': 'Error creating category',
      'error.updatingCategory': 'Error updating category',
      'error.deletingCategory': 'Error deleting category',
      'error.creatingUser': 'Error creating user',
      'error.updatingUser': 'Error updating user',
      'error.deletingUser': 'Error deleting user',
      'error.executingMovement': 'Error executing movement',
      'error.generatingStockReport': 'Error generating stock report',
      'error.generatingLowStockReport': 'Error generating low stock report',
      'error.generatingMovementReport': 'Error generating movement report',
      'error.accessDenied': 'Access denied. SuperAdmin role required.',
      'error.selectProductQuantity': 'Please select a product and enter a valid quantity.',
      'error.usernameEmailRequired': 'Username and email are required.',
      'error.passwordRequired': 'Password is required for new users.',
      'error.movementSuccess': 'Stock movement completed successfully!',
      
      // Pagination
      'pagination.showing': 'Showing',
      'pagination.to': 'to',
      'pagination.of': 'of',
      'pagination.first': 'First',
      'pagination.previous': 'Previous',
      'pagination.next': 'Next',
      'pagination.last': 'Last',
      'pagination.perPage': 'per page',
      
      // Language
      'language.english': 'English',
      'language.arabic': 'العربية'
    },
    ar: {
      // Navigation
      'nav.products': 'المنتجات',
      'nav.categories': 'الفئات',
      'nav.stockMovements': 'حركات المخزون',
      'nav.reports': 'التقارير',
      'nav.users': 'المستخدمون',
      'nav.logout': 'تسجيل الخروج',
      
      // Products
      'products.title': 'المنتجات',
      'products.add': 'إضافة منتج',
      'products.edit': 'تعديل منتج',
      'products.delete': 'حذف',
      'products.search': 'بحث',
      'products.sku': 'رمز المنتج',
      'products.name': 'الاسم',
      'products.description': 'الوصف',
      'products.category': 'الفئة',
      'products.quantity': 'الكمية',
      'products.price': 'السعر',
      'products.cost': 'التكلفة',
      'products.initialQuantity': 'الكمية الأولية',
      'products.minStockLevel': 'الحد الأدنى للمخزون',
      'products.barcode': 'الباركود',
      'products.lowStock': 'مخزون منخفض',
      'products.actions': 'الإجراءات',
      'products.noProducts': 'لم يتم العثور على منتجات',
      'products.none': 'لا شيء',
      
      // Categories
      'categories.title': 'الفئات',
      'categories.add': 'إضافة فئة',
      'categories.edit': 'تعديل فئة',
      'categories.delete': 'حذف',
      'categories.name': 'الاسم',
      'categories.description': 'الوصف',
      'categories.productCount': 'عدد المنتجات',
      'categories.actions': 'الإجراءات',
      'categories.noCategories': 'لم يتم العثور على فئات',
      
      // Stock Movements
      'stockMovements.title': 'حركات المخزون',
      'stockMovements.execute': 'تنفيذ الحركة',
      'stockMovements.product': 'المنتج',
      'stockMovements.type': 'النوع',
      'stockMovements.quantity': 'الكمية',
      'stockMovements.reason': 'السبب',
      'stockMovements.notes': 'ملاحظات',
      'stockMovements.history': 'سجل الحركات',
      'stockMovements.date': 'التاريخ',
      'stockMovements.createdBy': 'تم الإنشاء بواسطة',
      'stockMovements.noMovements': 'لم يتم العثور على حركات',
      'stockMovements.add': 'إضافة مخزون (زيادة الكمية)',
      'stockMovements.remove': 'إزالة مخزون (نقص الكمية)',
      'stockMovements.adjustment': 'تعديل (تعيين الكمية المطلقة)',
      'stockMovements.movementTypes': 'إضافة: تزييد المخزون بالكمية | إزالة: تقليل المخزون بالكمية | تعديل: تعيين المخزون بالكمية المطلقة',
      'stockMovements.selectProduct': 'اختر منتجاً...',
      
      // Reports
      'reports.title': 'التقارير',
      'reports.currentStock': 'المخزون الحالي',
      'reports.lowStock': 'تنبيهات المخزون المنخفض',
      'reports.movementHistory': 'سجل الحركات',
      'reports.export': 'تصدير إلى CSV',
      'reports.startDate': 'تاريخ البداية',
      'reports.endDate': 'تاريخ النهاية',
      'reports.generate': 'إنشاء تقرير',
      
      // Users
      'users.title': 'إدارة المستخدمين (للمشرف فقط)',
      'users.add': 'إضافة مستخدم',
      'users.edit': 'تعديل مستخدم',
      'users.delete': 'حذف',
      'users.username': 'اسم المستخدم',
      'users.email': 'البريد الإلكتروني',
      'users.password': 'كلمة المرور',
      'users.role': 'الدور',
      'users.active': 'نشط',
      'users.lastLogin': 'آخر تسجيل دخول',
      'users.actions': 'الإجراءات',
      'users.never': 'أبداً',
      'users.yes': 'نعم',
      'users.no': 'لا',
      
      // Common
      'common.save': 'حفظ',
      'common.cancel': 'إلغاء',
      'common.delete': 'حذف',
      'common.edit': 'تعديل',
      'common.search': 'بحث',
      'common.refresh': 'تحديث',
      'common.loading': 'جاري التحميل...',
      'common.error': 'خطأ',
      'common.success': 'نجح',
      'common.confirm': 'تأكيد',
      'common.yes': 'نعم',
      'common.no': 'لا',
      
      // Login
      'login.title': 'تسجيل الدخول',
      'login.appTitle': 'نظام إدارة المخزون',
      'login.username': 'اسم المستخدم',
      'login.password': 'كلمة المرور',
      'login.loggingIn': 'جاري تسجيل الدخول...',
      'login.defaultCredentials': 'البيانات الافتراضية:',
      'login.defaultUsername': 'اسم المستخدم: admin',
      'login.defaultPassword': 'كلمة المرور: admin123',
      'login.pleaseEnter': 'يرجى إدخال اسم المستخدم وكلمة المرور',
      'login.invalid': 'اسم المستخدم أو كلمة المرور غير صحيحة',
      
      // Logout
      'logout.confirm': 'هل أنت متأكد أنك تريد تسجيل الخروج؟',
      
      // Confirmations
      'confirm.deleteProduct': 'هل أنت متأكد أنك تريد حذف "{{name}}"؟',
      'confirm.deleteCategory': 'هل أنت متأكد أنك تريد حذف "{{name}}"؟ سيتم إزالة الفئة من جميع المنتجات.',
      'confirm.deleteUser': 'هل أنت متأكد أنك تريد حذف المستخدم \'{{username}}\'؟',
      
      // Error messages
      'error.loadingProducts': 'خطأ في تحميل المنتجات. تأكد من تشغيل API.',
      'error.loadingCategories': 'خطأ في تحميل الفئات. تأكد من تشغيل API.',
      'error.loadingMovements': 'خطأ في تحميل الحركات. تأكد من تشغيل API.',
      'error.loadingUsers': 'خطأ في تحميل المستخدمين. تأكد من تشغيل API.',
      'error.creatingProduct': 'خطأ في إنشاء المنتج',
      'error.updatingProduct': 'خطأ في تحديث المنتج',
      'error.deletingProduct': 'خطأ في حذف المنتج',
      'error.creatingCategory': 'خطأ في إنشاء الفئة',
      'error.updatingCategory': 'خطأ في تحديث الفئة',
      'error.deletingCategory': 'خطأ في حذف الفئة',
      'error.creatingUser': 'خطأ في إنشاء المستخدم',
      'error.updatingUser': 'خطأ في تحديث المستخدم',
      'error.deletingUser': 'خطأ في حذف المستخدم',
      'error.executingMovement': 'خطأ في تنفيذ الحركة',
      'error.generatingStockReport': 'خطأ في إنشاء تقرير المخزون',
      'error.generatingLowStockReport': 'خطأ في إنشاء تقرير المخزون المنخفض',
      'error.generatingMovementReport': 'خطأ في إنشاء تقرير الحركات',
      'error.accessDenied': 'تم رفض الوصول. يتطلب دور المشرف.',
      'error.selectProductQuantity': 'يرجى اختيار منتج وإدخال كمية صحيحة.',
      'error.usernameEmailRequired': 'اسم المستخدم والبريد الإلكتروني مطلوبان.',
      'error.passwordRequired': 'كلمة المرور مطلوبة للمستخدمين الجدد.',
      'error.movementSuccess': 'تم تنفيذ حركة المخزون بنجاح!',
      
      // Pagination
      'pagination.showing': 'عرض',
      'pagination.to': 'إلى',
      'pagination.of': 'من',
      'pagination.first': 'الأول',
      'pagination.previous': 'السابق',
      'pagination.next': 'التالي',
      'pagination.last': 'الأخير',
      'pagination.perPage': 'لكل صفحة',
      
      // Language
      'language.english': 'English',
      'language.arabic': 'العربية'
    }
  };

  constructor(private localeService: LocaleService) {}

  translate(key: string): string {
    const locale = this.localeService.getCurrentLocale();
    return this.translations[locale][key] || key;
  }

  translateWithParams(key: string, params: { [key: string]: string | number }): string {
    let translation = this.translate(key);
    Object.keys(params).forEach(param => {
      translation = translation.replace(`{{${param}}}`, String(params[param]));
    });
    return translation;
  }
}

