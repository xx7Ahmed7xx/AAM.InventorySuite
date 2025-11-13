using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Enums;
using Inventory.Desktop.Services;
using Inventory.Desktop;

namespace Inventory.Desktop.ViewModels;

public class StockMovementViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private readonly AuthResponseDto? _authResponse;
    private StockMovementDto? _selectedMovement;
    private ProductDto? _selectedProduct;
    private ObservableCollection<StockMovementDto> _movements = new();
    private ObservableCollection<ProductDto> _products = new();
    private ObservableCollection<MovementTypeDisplayItem> _movementTypes = new();
    private MovementTypeDisplayItem? _selectedMovementTypeItem;
    private int _quantity = 1;
    private string _reason = string.Empty;
    private string _notes = string.Empty;
    
    // Pagination
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalCount = 0;
    private int _totalPages = 0;
    private bool _hasPreviousPage = false;
    private bool _hasNextPage = false;

    public StockMovementViewModel(ApiClient apiClient, AuthResponseDto? authResponse = null)
    {
        _apiClient = apiClient;
        _authResponse = authResponse;

        // Initialize movement types with descriptions
        UpdateMovementTypes();
        _selectedMovementTypeItem = _movementTypes.First();
        
        // Subscribe to culture changes to update movement type names and pagination info
        if (App.LocalizationService != null)
        {
            App.LocalizationService.CultureChanged += (sender, e) =>
            {
                UpdateMovementTypes();
                OnPropertyChanged(nameof(PaginationInfo));
            };
        }

        LoadMovementsCommand = new RelayCommand(async _ => await LoadMovementsAsync());
        LoadProductsCommand = new RelayCommand(async _ => await LoadProductsAsync());
        ExecuteMovementCommand = new RelayCommand(async _ => await ExecuteMovementAsync(), _ => SelectedProduct != null && Quantity > 0);
        RefreshCommand = new RelayCommand(async _ => 
        {
            await LoadMovementsAsync();
            await LoadProductsAsync();
        });
        FirstPageCommand = new RelayCommand(async _ => await GoToPageAsync(1), _ => HasPreviousPage);
        PreviousPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => HasPreviousPage);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => HasNextPage);
        LastPageCommand = new RelayCommand(async _ => await GoToPageAsync(TotalPages), _ => HasNextPage);
        ChangePageSizeCommand = new RelayCommand(async _ => await ChangePageSizeAsync());

        _ = LoadMovementsAsync();
        _ = LoadProductsAsync();
    }

    public ObservableCollection<StockMovementDto> Movements
    {
        get => _movements;
        set => SetProperty(ref _movements, value);
    }

    public ObservableCollection<ProductDto> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public StockMovementDto? SelectedMovement
    {
        get => _selectedMovement;
        set => SetProperty(ref _selectedMovement, value);
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

    public ObservableCollection<MovementTypeDisplayItem> MovementTypes
    {
        get => _movementTypes;
        set => SetProperty(ref _movementTypes, value);
    }

    public MovementTypeDisplayItem? SelectedMovementTypeItem
    {
        get => _selectedMovementTypeItem;
        set
        {
            SetProperty(ref _selectedMovementTypeItem, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            SetProperty(ref _quantity, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Reason
    {
        get => _reason;
        set => SetProperty(ref _reason, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public ICommand LoadMovementsCommand { get; }
    public ICommand LoadProductsCommand { get; }
    public ICommand ExecuteMovementCommand { get; }
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
                if (_pageSize > 0)
                {
                    CurrentPage = 1;
                    _ = LoadMovementsAsync();
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
                return $"Showing {((CurrentPage - 1) * PageSize + 1)} to {Math.Min(CurrentPage * PageSize, TotalCount)} of {TotalCount} movements";
            
            var showing = App.TranslationService.Translate("pagination.showing");
            var to = App.TranslationService.Translate("pagination.to");
            var of = App.TranslationService.Translate("pagination.of");
            var movements = App.TranslationService.Translate("nav.stockMovements");
            return $"{showing} {((CurrentPage - 1) * PageSize + 1)} {to} {Math.Min(CurrentPage * PageSize, TotalCount)} {of} {TotalCount} {movements}";
        }
    }

    private async Task LoadMovementsAsync()
    {
        try
        {
            var result = await _apiClient.GetStockMovementsPagedAsync(CurrentPage, PageSize);
            Movements = new ObservableCollection<StockMovementDto>(result.Items);
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasPreviousPage = result.HasPreviousPage;
            HasNextPage = result.HasNextPage;
            OnPropertyChanged(nameof(PaginationInfo));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading movements: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async Task GoToPageAsync(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await LoadMovementsAsync();
        }
    }
    
    private async Task ChangePageSizeAsync()
    {
        CurrentPage = 1;
        await LoadMovementsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _apiClient.GetProductsAsync();
            Products = new ObservableCollection<ProductDto>(products);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task ExecuteMovementAsync()
    {
        if (SelectedProduct == null || Quantity <= 0)
        {
            MessageBox.Show("Please select a product and enter a valid quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (SelectedMovementTypeItem == null)
            {
                MessageBox.Show("Please select a movement type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new StockMovementRequestDto
            {
                ProductId = SelectedProduct.Id,
                MovementType = (int)SelectedMovementTypeItem.Type,
                Quantity = Quantity,
                Reason = Reason,
                Notes = Notes,
                CreatedBy = _authResponse?.Username ?? _authResponse?.Email ?? "Unknown"
            };

            StockMovementDto movement;
            switch (SelectedMovementTypeItem.Type)
            {
                case StockMovementType.Add:
                    movement = await _apiClient.AddStockAsync(request);
                    break;
                case StockMovementType.Remove:
                    movement = await _apiClient.RemoveStockAsync(request);
                    break;
                case StockMovementType.Adjustment:
                    movement = await _apiClient.AdjustStockAsync(request);
                    break;
                default:
                    throw new ArgumentException("Invalid movement type");
            }

            MessageBox.Show("Stock movement completed successfully!", 
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset form
            Quantity = 1;
            Reason = string.Empty;
            Notes = string.Empty;

            // Refresh data
            CurrentPage = 1;
            await LoadMovementsAsync();
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error executing movement: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateMovementTypes()
    {
        if (App.TranslationService == null)
        {
            _movementTypes = new ObservableCollection<MovementTypeDisplayItem>
            {
                new MovementTypeDisplayItem { Type = StockMovementType.Add, DisplayName = "Add Stock (increase by quantity)" },
                new MovementTypeDisplayItem { Type = StockMovementType.Remove, DisplayName = "Remove Stock (decrease by quantity)" },
                new MovementTypeDisplayItem { Type = StockMovementType.Adjustment, DisplayName = "Adjustment (set to absolute quantity)" }
            };
        }
        else
        {
            var addText = App.TranslationService.Translate("stockMovements.add");
            var removeText = App.TranslationService.Translate("stockMovements.remove");
            var adjustmentText = App.TranslationService.Translate("stockMovements.adjustment");
            
            _movementTypes = new ObservableCollection<MovementTypeDisplayItem>
            {
                new MovementTypeDisplayItem { Type = StockMovementType.Add, DisplayName = addText },
                new MovementTypeDisplayItem { Type = StockMovementType.Remove, DisplayName = removeText },
                new MovementTypeDisplayItem { Type = StockMovementType.Adjustment, DisplayName = adjustmentText }
            };
        }
        
        // Preserve selected item if possible
        if (_selectedMovementTypeItem != null)
        {
            var selectedType = _selectedMovementTypeItem.Type;
            _selectedMovementTypeItem = _movementTypes.FirstOrDefault(m => m.Type == selectedType) ?? _movementTypes.First();
        }
        else
        {
            _selectedMovementTypeItem = _movementTypes.First();
        }
        
        OnPropertyChanged(nameof(MovementTypes));
        OnPropertyChanged(nameof(SelectedMovementTypeItem));
    }

    // Helper class for displaying movement types with descriptions
    public class MovementTypeDisplayItem
    {
        public StockMovementType Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
