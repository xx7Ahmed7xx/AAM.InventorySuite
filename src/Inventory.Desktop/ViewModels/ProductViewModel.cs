using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.ViewModels;

public class ProductViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private readonly AuthResponseDto? _authResponse;
    private ProductDto? _selectedProduct;
    private string _searchTerm = string.Empty;
    private ObservableCollection<ProductDto> _products = new();
    private ObservableCollection<CategoryDto> _categories = new();
    
    // Pagination
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalCount = 0;
    private int _totalPages = 0;
    private bool _hasPreviousPage = false;
    private bool _hasNextPage = false;

    public ProductViewModel(ApiClient apiClient, AuthResponseDto? authResponse = null)
    {
        _apiClient = apiClient;
        _authResponse = authResponse;

        LoadProductsCommand = new RelayCommand(async _ => await LoadProductsAsync());
        LoadCategoriesCommand = new RelayCommand(async _ => await LoadCategoriesAsync());
        AddProductCommand = new RelayCommand(_ => AddProduct());
        EditProductCommand = new RelayCommand(_ => EditProduct(), _ => SelectedProduct != null);
        DeleteProductCommand = new RelayCommand(async _ => await DeleteProductAsync(), _ => SelectedProduct != null);
        SearchCommand = new RelayCommand(async _ => await SearchProductsAsync());
        RefreshCommand = new RelayCommand(async _ => await LoadProductsAsync());
        FirstPageCommand = new RelayCommand(async _ => await GoToPageAsync(1), _ => HasPreviousPage);
        PreviousPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => HasPreviousPage);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => HasNextPage);
        LastPageCommand = new RelayCommand(async _ => await GoToPageAsync(TotalPages), _ => HasNextPage);
        ChangePageSizeCommand = new RelayCommand(async _ => await ChangePageSizeAsync());

        // Subscribe to culture changes to update pagination info
        if (App.LocalizationService != null)
        {
            App.LocalizationService.CultureChanged += (sender, e) => OnPropertyChanged(nameof(PaginationInfo));
        }

        _ = LoadProductsAsync();
        _ = LoadCategoriesAsync();
    }

    public ObservableCollection<ProductDto> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ObservableCollection<CategoryDto> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            SetProperty(ref _selectedProduct, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    public ICommand LoadProductsCommand { get; }
    public ICommand LoadCategoriesCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand DeleteProductCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand ChangePageSizeCommand { get; }
    
    // Pagination Properties
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                OnPropertyChanged(nameof(PaginationInfo));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
    
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
            {
                // Auto-apply page size change
                if (_pageSize > 0)
                {
                    CurrentPage = 1;
                    _ = LoadProductsAsync();
                }
            }
        }
    }
    
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (SetProperty(ref _totalCount, value))
            {
                OnPropertyChanged(nameof(PaginationInfo));
            }
        }
    }
    
    public int TotalPages
    {
        get => _totalPages;
        set
        {
            SetProperty(ref _totalPages, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool HasPreviousPage
    {
        get => _hasPreviousPage;
        set
        {
            SetProperty(ref _hasPreviousPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool HasNextPage
    {
        get => _hasNextPage;
        set
        {
            SetProperty(ref _hasNextPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public string PaginationInfo
    {
        get
        {
            if (App.TranslationService == null)
                return $"Showing {((CurrentPage - 1) * PageSize + 1)} to {Math.Min(CurrentPage * PageSize, TotalCount)} of {TotalCount} products";
            
            var showing = App.TranslationService.Translate("pagination.showing");
            var to = App.TranslationService.Translate("pagination.to");
            var of = App.TranslationService.Translate("pagination.of");
            var products = App.TranslationService.Translate("nav.products");
            return $"{showing} {((CurrentPage - 1) * PageSize + 1)} {to} {Math.Min(CurrentPage * PageSize, TotalCount)} {of} {TotalCount} {products}";
        }
    }

    /// <summary>
    /// Returns true if the current user is Moderator or SuperAdmin
    /// </summary>
    public bool IsModerator => _authResponse != null && 
        (_authResponse.Role == "Moderator" || _authResponse.Role == "SuperAdmin");

    private async Task LoadProductsAsync()
    {
        try
        {
            var result = await _apiClient.GetProductsPagedAsync(CurrentPage, PageSize);
            Products = new ObservableCollection<ProductDto>(result.Items);
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasPreviousPage = result.HasPreviousPage;
            HasNextPage = result.HasNextPage;
            OnPropertyChanged(nameof(PaginationInfo));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async Task GoToPageAsync(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await LoadProductsAsync();
        }
    }
    
    private async Task ChangePageSizeAsync()
    {
        // PageSize is already updated via binding
        CurrentPage = 1;
        await LoadProductsAsync();
    }
    
    public void OnPageSizeChanged()
    {
        CurrentPage = 1;
        _ = LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _apiClient.GetCategoriesAsync();
            Categories = new ObservableCollection<CategoryDto>(categories);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task SearchProductsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            CurrentPage = 1;
            await LoadProductsAsync();
            return;
        }

        try
        {
            var products = await _apiClient.SearchProductsAsync(SearchTerm);
            Products = new ObservableCollection<ProductDto>(products);
            TotalCount = products.Count;
            TotalPages = 1;
            HasPreviousPage = false;
            HasNextPage = false;
            OnPropertyChanged(nameof(PaginationInfo));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddProduct()
    {
        var dialog = new Views.ProductDialog(_apiClient);
        if (dialog.ShowDialog() == true)
        {
            CurrentPage = 1;
            _ = LoadProductsAsync();
        }
    }

    private void EditProduct()
    {
        if (SelectedProduct == null) return;

        var dialog = new Views.ProductDialog(_apiClient, SelectedProduct);
        if (dialog.ShowDialog() == true)
        {
            CurrentPage = 1;
            _ = LoadProductsAsync();
        }
    }

    private async Task DeleteProductAsync()
    {
        if (SelectedProduct == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{SelectedProduct.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.DeleteProductAsync(SelectedProduct.Id);
                CurrentPage = 1;
                await LoadProductsAsync();
                MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
