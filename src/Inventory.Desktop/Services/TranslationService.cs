using System;
using System.Collections.Generic;
using System.Globalization;

namespace Inventory.Desktop.Services;

/// <summary>
/// Simple translation service for WPF application
/// </summary>
public class TranslationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        {
            "en-US", new Dictionary<string, string>
            {
                  // Login
                  { "login.title", "Login" },
                  { "login.appTitle", "Inventory Suite" },
                  { "login.defaultCredentials", "Default: admin / admin123" },
                  
                  // Logout
                  { "logout.confirm", "Are you sure you want to logout?" },
                
                // Menu
                { "menu.file", "File" },
                { "menu.view", "View" },
                { "menu.exit", "Exit" },
                
                // Navigation
                { "nav.products", "Products" },
                { "nav.categories", "Categories" },
                { "nav.stockMovements", "Stock Movements" },
                { "nav.reports", "Reports" },
                { "nav.users", "Users" },
                { "nav.logout", "Logout" },
                
                // Common
                { "common.save", "Save" },
                { "common.cancel", "Cancel" },
                { "common.delete", "Delete" },
                { "common.edit", "Edit" },
                { "common.refresh", "Refresh" },
                { "common.yes", "Yes" },
                { "common.no", "No" },
                { "common.never", "Never" },
                
                // Products
                { "products.title", "Products" },
                { "products.add", "Add Product" },
                { "products.edit", "Edit Product" },
                { "products.delete", "Delete" },
                { "products.search", "Search" },
                { "products.sku", "SKU" },
                { "products.name", "Name" },
                { "products.category", "Category" },
                { "products.quantity", "Quantity" },
                { "products.price", "Price" },
                { "products.cost", "Cost" },
                { "products.initialQuantity", "Initial Quantity" },
                { "products.minStockLevel", "Min Stock Level" },
                { "products.barcode", "Barcode" },
                { "products.description", "Description" },
                { "products.lowStock", "Low Stock" },
                { "products.actions", "Actions" },
                { "products.none", "None" },
                { "products.total", "Total Products" },
                
                // Categories
                { "categories.title", "Categories" },
                { "categories.add", "Add Category" },
                { "categories.edit", "Edit Category" },
                { "categories.delete", "Delete" },
                { "categories.name", "Name" },
                { "categories.description", "Description" },
                { "categories.productCount", "Product Count" },
                { "categories.actions", "Actions" },
                { "categories.total", "Total Categories" },
                
                // Stock Movements
                { "stockMovements.title", "Stock Movements" },
                { "stockMovements.execute", "Execute Movement" },
                { "stockMovements.product", "Product" },
                { "stockMovements.type", "Movement Type" },
                { "stockMovements.quantity", "Quantity" },
                { "stockMovements.reason", "Reason" },
                { "stockMovements.notes", "Notes" },
                { "stockMovements.history", "Movement History" },
                { "stockMovements.date", "Date" },
                { "stockMovements.createdBy", "Created By" },
                { "stockMovements.productLabel", "Product" },
                { "stockMovements.movementTypeLabel", "Movement Type" },
                { "stockMovements.quantityLabel", "Quantity" },
                { "stockMovements.reasonLabel", "Reason" },
                { "stockMovements.notesLabel", "Notes" },
                { "stockMovements.movementTypesHelp", "Add: increases stock by quantity | Remove: decreases stock by quantity | Adjustment: sets stock to absolute quantity" },
                { "stockMovements.add", "Add Stock (increase by quantity)" },
                { "stockMovements.remove", "Remove Stock (decrease by quantity)" },
                { "stockMovements.adjustment", "Adjustment (set to absolute quantity)" },
                
                // Reports
                { "reports.title", "Reports" },
                { "reports.currentStock", "Current Stock" },
                { "reports.lowStock", "Low Stock Alerts" },
                { "reports.movementHistory", "Movement History" },
                { "reports.export", "Export to CSV" },
                { "reports.startDate", "Start Date" },
                { "reports.endDate", "End Date" },
                { "reports.generate", "Generate Report" },
                { "reports.noData", "No data available" },
                
                // Users
                { "users.title", "User Management" },
                { "users.add", "Add User" },
                { "users.edit", "Edit User" },
                { "users.username", "Username" },
                { "users.email", "Email" },
                { "users.password", "Password" },
                { "users.role", "Role" },
                { "users.active", "Active" },
                { "users.lastLogin", "Last Login" },
                { "users.actions", "Actions" },
                { "users.total", "Total Users" },
                
                // Pagination
                { "pagination.pageSize", "Page Size" },
                { "pagination.first", "First" },
                { "pagination.previous", "Previous" },
                { "pagination.next", "Next" },
                { "pagination.last", "Last" },
                { "pagination.page", "Page" },
                { "pagination.of", "of" },
                { "pagination.apply", "Apply" },
                { "pagination.showing", "Showing" },
                { "pagination.to", "to" }
            }
        },
        {
            "ar-SA", new Dictionary<string, string>
            {
                // Navigation
                { "nav.products", "المنتجات" },
                { "nav.categories", "الفئات" },
                { "nav.stockMovements", "حركات المخزون" },
                { "nav.reports", "التقارير" },
                { "nav.users", "المستخدمون" },
                { "nav.logout", "تسجيل الخروج" },
                
                  // Login
                  { "login.title", "تسجيل الدخول" },
                  { "login.appTitle", "نظام إدارة المخزون" },
                  { "login.defaultCredentials", "افتراضي: admin / admin123" },
                  
                  // Logout
                  { "logout.confirm", "هل أنت متأكد أنك تريد تسجيل الخروج؟" },
                
                // Menu
                { "menu.file", "ملف" },
                { "menu.view", "عرض" },
                { "menu.exit", "خروج" },
                
                // Common
                { "common.save", "حفظ" },
                { "common.cancel", "إلغاء" },
                { "common.delete", "حذف" },
                { "common.edit", "تعديل" },
                { "common.refresh", "تحديث" },
                { "common.yes", "نعم" },
                { "common.no", "لا" },
                { "common.never", "أبداً" },
                
                // Products
                { "products.title", "المنتجات" },
                { "products.add", "إضافة منتج" },
                { "products.edit", "تعديل منتج" },
                { "products.delete", "حذف" },
                { "products.search", "بحث" },
                { "products.sku", "رمز المنتج" },
                { "products.name", "الاسم" },
                { "products.category", "الفئة" },
                { "products.quantity", "الكمية" },
                { "products.price", "السعر" },
                { "products.cost", "التكلفة" },
                { "products.initialQuantity", "الكمية الأولية" },
                { "products.minStockLevel", "الحد الأدنى للمخزون" },
                { "products.barcode", "الباركود" },
                { "products.description", "الوصف" },
                { "products.lowStock", "مخزون منخفض" },
                { "products.actions", "الإجراءات" },
                { "products.none", "لا شيء" },
                { "products.total", "إجمالي المنتجات" },
                
                // Categories
                { "categories.title", "الفئات" },
                { "categories.add", "إضافة فئة" },
                { "categories.edit", "تعديل فئة" },
                { "categories.delete", "حذف" },
                { "categories.name", "الاسم" },
                { "categories.description", "الوصف" },
                { "categories.productCount", "عدد المنتجات" },
                { "categories.actions", "الإجراءات" },
                { "categories.total", "إجمالي الفئات" },
                
                // Stock Movements
                { "stockMovements.title", "حركات المخزون" },
                { "stockMovements.execute", "تنفيذ الحركة" },
                { "stockMovements.product", "المنتج" },
                { "stockMovements.type", "نوع الحركة" },
                { "stockMovements.quantity", "الكمية" },
                { "stockMovements.reason", "السبب" },
                { "stockMovements.notes", "ملاحظات" },
                { "stockMovements.history", "سجل الحركات" },
                { "stockMovements.date", "التاريخ" },
                { "stockMovements.createdBy", "تم الإنشاء بواسطة" },
                { "stockMovements.productLabel", "المنتج" },
                { "stockMovements.movementTypeLabel", "نوع الحركة" },
                { "stockMovements.quantityLabel", "الكمية" },
                { "stockMovements.reasonLabel", "السبب" },
                { "stockMovements.notesLabel", "ملاحظات" },
                { "stockMovements.movementTypesHelp", "إضافة: تزييد المخزون بالكمية | إزالة: تقليل المخزون بالكمية | تعديل: تعيين المخزون بالكمية المطلقة" },
                { "stockMovements.add", "إضافة مخزون (زيادة الكمية)" },
                { "stockMovements.remove", "إزالة مخزون (نقص الكمية)" },
                { "stockMovements.adjustment", "تعديل (تعيين الكمية المطلقة)" },
                
                // Reports
                { "reports.title", "التقارير" },
                { "reports.currentStock", "المخزون الحالي" },
                { "reports.lowStock", "تنبيهات المخزون المنخفض" },
                { "reports.movementHistory", "سجل الحركات" },
                { "reports.export", "تصدير إلى CSV" },
                { "reports.startDate", "تاريخ البدء" },
                { "reports.endDate", "تاريخ الانتهاء" },
                { "reports.generate", "إنشاء تقرير" },
                { "reports.noData", "لا توجد بيانات متاحة" },
                
                // Users
                { "users.title", "إدارة المستخدمين" },
                { "users.add", "إضافة مستخدم" },
                { "users.edit", "تعديل مستخدم" },
                { "users.username", "اسم المستخدم" },
                { "users.email", "البريد الإلكتروني" },
                { "users.password", "كلمة المرور" },
                { "users.role", "الدور" },
                { "users.active", "نشط" },
                { "users.lastLogin", "آخر تسجيل دخول" },
                { "users.actions", "الإجراءات" },
                { "users.total", "إجمالي المستخدمين" },
                
                // Pagination
                { "pagination.pageSize", "حجم الصفحة" },
                { "pagination.first", "الأول" },
                { "pagination.previous", "السابق" },
                { "pagination.next", "التالي" },
                { "pagination.last", "الأخير" },
                { "pagination.page", "صفحة" },
                { "pagination.of", "من" },
                { "pagination.apply", "تطبيق" },
                { "pagination.showing", "عرض" },
                { "pagination.to", "إلى" }
            }
        }
    };

    private CultureInfo _currentCulture;

    public TranslationService()
    {
        _currentCulture = CultureInfo.CurrentCulture;
    }

    public TranslationService(CultureInfo culture)
    {
        _currentCulture = culture;
    }

    public string Translate(string key)
    {
        var cultureName = _currentCulture.Name;
        
        // Fallback to base culture if specific culture not found
        if (!_translations.ContainsKey(cultureName))
        {
            cultureName = _currentCulture.TwoLetterISOLanguageName == "ar" ? "ar-SA" : "en-US";
        }

        if (_translations.TryGetValue(cultureName, out var cultureTranslations))
        {
            if (cultureTranslations.TryGetValue(key, out var translation))
            {
                return translation;
            }
        }

        // Return key if translation not found
        return key;
    }

    public void SetCulture(CultureInfo culture)
    {
        _currentCulture = culture;
    }
}

