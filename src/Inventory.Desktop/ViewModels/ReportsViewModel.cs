using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;
using Microsoft.Win32;
using Inventory.Desktop;

namespace Inventory.Desktop.ViewModels;

public class ReportsViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private readonly AuthResponseDto? _authResponse;
    private ObservableCollection<ProductDto> _stockReport = new();
    private ObservableCollection<ProductDto> _lowStockReport = new();
    private ObservableCollection<StockMovementDto> _movementReport = new();
    private DateTime _startDate = DateTime.Today.AddDays(-30);
    private DateTime _endDate = DateTime.Today;
    
    // Pagination for stock report
    private int _stockCurrentPage = 1;
    private int _stockPageSize = 10;
    private int _stockTotalCount = 0;
    private int _stockTotalPages = 0;
    private bool _stockHasPreviousPage = false;
    private bool _stockHasNextPage = false;
    
    // Pagination for low stock report
    private int _lowStockCurrentPage = 1;
    private int _lowStockPageSize = 10;
    private int _lowStockTotalCount = 0;
    private int _lowStockTotalPages = 0;
    private bool _lowStockHasPreviousPage = false;
    private bool _lowStockHasNextPage = false;

    public ReportsViewModel(ApiClient apiClient, AuthResponseDto? authResponse = null)
    {
        _apiClient = apiClient;
        _authResponse = authResponse;

        GenerateStockReportCommand = new RelayCommand(async _ => await GenerateStockReportAsync());
        GenerateLowStockReportCommand = new RelayCommand(async _ => await GenerateLowStockReportAsync());
        GenerateMovementReportCommand = new RelayCommand(async _ => await GenerateMovementReportAsync());
        ExportStockReportCommand = new RelayCommand(_ => ExportStockReport(), _ => StockReport.Count > 0);
        ExportLowStockReportCommand = new RelayCommand(_ => ExportLowStockReport(), _ => LowStockReport.Count > 0);
        ExportMovementReportCommand = new RelayCommand(_ => ExportMovementReport(), _ => MovementReport.Count > 0);
        StockFirstPageCommand = new RelayCommand(async _ => await StockGoToPageAsync(1), _ => StockHasPreviousPage);
        StockPreviousPageCommand = new RelayCommand(async _ => await StockGoToPageAsync(StockCurrentPage - 1), _ => StockHasPreviousPage);
        StockNextPageCommand = new RelayCommand(async _ => await StockGoToPageAsync(StockCurrentPage + 1), _ => StockHasNextPage);
        StockLastPageCommand = new RelayCommand(async _ => await StockGoToPageAsync(StockTotalPages), _ => StockHasNextPage);
        StockChangePageSizeCommand = new RelayCommand(async _ => await StockChangePageSizeAsync());
        LowStockFirstPageCommand = new RelayCommand(async _ => await LowStockGoToPageAsync(1), _ => LowStockHasPreviousPage);
        LowStockPreviousPageCommand = new RelayCommand(async _ => await LowStockGoToPageAsync(LowStockCurrentPage - 1), _ => LowStockHasPreviousPage);
        LowStockNextPageCommand = new RelayCommand(async _ => await LowStockGoToPageAsync(LowStockCurrentPage + 1), _ => LowStockHasNextPage);
        LowStockLastPageCommand = new RelayCommand(async _ => await LowStockGoToPageAsync(LowStockTotalPages), _ => LowStockHasNextPage);
        LowStockChangePageSizeCommand = new RelayCommand(async _ => await LowStockChangePageSizeAsync());

        // Subscribe to culture changes to update pagination info
        if (App.LocalizationService != null)
        {
            App.LocalizationService.CultureChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(StockPaginationInfo));
                OnPropertyChanged(nameof(LowStockPaginationInfo));
            };
        }
    }

    public ObservableCollection<ProductDto> StockReport
    {
        get => _stockReport;
        set => SetProperty(ref _stockReport, value);
    }

    public ObservableCollection<ProductDto> LowStockReport
    {
        get => _lowStockReport;
        set => SetProperty(ref _lowStockReport, value);
    }

    public ObservableCollection<StockMovementDto> MovementReport
    {
        get => _movementReport;
        set => SetProperty(ref _movementReport, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public ICommand GenerateStockReportCommand { get; }
    public ICommand GenerateLowStockReportCommand { get; }
    public ICommand GenerateMovementReportCommand { get; }
    public ICommand ExportStockReportCommand { get; }
    public ICommand ExportLowStockReportCommand { get; }
    public ICommand ExportMovementReportCommand { get; }
    public ICommand StockFirstPageCommand { get; }
    public ICommand StockPreviousPageCommand { get; }
    public ICommand StockNextPageCommand { get; }
    public ICommand StockLastPageCommand { get; }
    public ICommand StockChangePageSizeCommand { get; }
    public ICommand LowStockFirstPageCommand { get; }
    public ICommand LowStockPreviousPageCommand { get; }
    public ICommand LowStockNextPageCommand { get; }
    public ICommand LowStockLastPageCommand { get; }
    public ICommand LowStockChangePageSizeCommand { get; }
    
    // Pagination Properties for Stock Report
    public int StockCurrentPage
    {
        get => _stockCurrentPage;
        set
        {
            if (SetProperty(ref _stockCurrentPage, value))
            {
                OnPropertyChanged(nameof(StockPaginationInfo));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
    
    public int StockPageSize
    {
        get => _stockPageSize;
        set
        {
            if (SetProperty(ref _stockPageSize, value))
            {
                if (_stockPageSize > 0)
                {
                    StockCurrentPage = 1;
                    _ = GenerateStockReportAsync();
                }
            }
        }
    }
    
    public int StockTotalCount
    {
        get => _stockTotalCount;
        set
        {
            if (SetProperty(ref _stockTotalCount, value))
            {
                OnPropertyChanged(nameof(StockPaginationInfo));
            }
        }
    }
    
    public int StockTotalPages
    {
        get => _stockTotalPages;
        set
        {
            SetProperty(ref _stockTotalPages, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool StockHasPreviousPage
    {
        get => _stockHasPreviousPage;
        set
        {
            SetProperty(ref _stockHasPreviousPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool StockHasNextPage
    {
        get => _stockHasNextPage;
        set
        {
            SetProperty(ref _stockHasNextPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public string StockPaginationInfo
    {
        get
        {
            if (App.TranslationService == null)
                return $"Showing {((StockCurrentPage - 1) * StockPageSize + 1)} to {Math.Min(StockCurrentPage * StockPageSize, StockTotalCount)} of {StockTotalCount} products";
            
            var showing = App.TranslationService.Translate("pagination.showing");
            var to = App.TranslationService.Translate("pagination.to");
            var of = App.TranslationService.Translate("pagination.of");
            var products = App.TranslationService.Translate("nav.products");
            return $"{showing} {((StockCurrentPage - 1) * StockPageSize + 1)} {to} {Math.Min(StockCurrentPage * StockPageSize, StockTotalCount)} {of} {StockTotalCount} {products}";
        }
    }
    
    // Pagination Properties for Low Stock Report
    public int LowStockCurrentPage
    {
        get => _lowStockCurrentPage;
        set
        {
            if (SetProperty(ref _lowStockCurrentPage, value))
            {
                OnPropertyChanged(nameof(LowStockPaginationInfo));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
    
    public int LowStockPageSize
    {
        get => _lowStockPageSize;
        set
        {
            if (SetProperty(ref _lowStockPageSize, value))
            {
                if (_lowStockPageSize > 0)
                {
                    LowStockCurrentPage = 1;
                    _ = GenerateLowStockReportAsync();
                }
            }
        }
    }
    
    public int LowStockTotalCount
    {
        get => _lowStockTotalCount;
        set
        {
            if (SetProperty(ref _lowStockTotalCount, value))
            {
                OnPropertyChanged(nameof(LowStockPaginationInfo));
            }
        }
    }
    
    public int LowStockTotalPages
    {
        get => _lowStockTotalPages;
        set
        {
            SetProperty(ref _lowStockTotalPages, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool LowStockHasPreviousPage
    {
        get => _lowStockHasPreviousPage;
        set
        {
            SetProperty(ref _lowStockHasPreviousPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public bool LowStockHasNextPage
    {
        get => _lowStockHasNextPage;
        set
        {
            SetProperty(ref _lowStockHasNextPage, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public string LowStockPaginationInfo
    {
        get
        {
            if (App.TranslationService == null)
                return $"Showing {((LowStockCurrentPage - 1) * LowStockPageSize + 1)} to {Math.Min(LowStockCurrentPage * LowStockPageSize, LowStockTotalCount)} of {LowStockTotalCount} products";
            
            var showing = App.TranslationService.Translate("pagination.showing");
            var to = App.TranslationService.Translate("pagination.to");
            var of = App.TranslationService.Translate("pagination.of");
            var products = App.TranslationService.Translate("nav.products");
            return $"{showing} {((LowStockCurrentPage - 1) * LowStockPageSize + 1)} {to} {Math.Min(LowStockCurrentPage * LowStockPageSize, LowStockTotalCount)} {of} {LowStockTotalCount} {products}";
        }
    }

    /// <summary>
    /// Returns true if the current user is Moderator or SuperAdmin
    /// </summary>
    public bool IsModerator => _authResponse != null && 
        (_authResponse.Role == "Moderator" || _authResponse.Role == "SuperAdmin");

    private async Task GenerateStockReportAsync()
    {
        try
        {
            var result = await _apiClient.GetStockReportPagedAsync(StockCurrentPage, StockPageSize);
            StockReport = new ObservableCollection<ProductDto>(result.Items);
            StockTotalCount = result.TotalCount;
            StockTotalPages = result.TotalPages;
            StockHasPreviousPage = result.HasPreviousPage;
            StockHasNextPage = result.HasNextPage;
            OnPropertyChanged(nameof(StockPaginationInfo));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating stock report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async Task StockGoToPageAsync(int page)
    {
        if (page >= 1 && page <= StockTotalPages)
        {
            StockCurrentPage = page;
            await GenerateStockReportAsync();
        }
    }
    
    private async Task StockChangePageSizeAsync()
    {
        StockCurrentPage = 1;
        await GenerateStockReportAsync();
    }

    private async Task GenerateLowStockReportAsync()
    {
        try
        {
            var result = await _apiClient.GetLowStockReportPagedAsync(LowStockCurrentPage, LowStockPageSize);
            LowStockReport = new ObservableCollection<ProductDto>(result.Items);
            LowStockTotalCount = result.TotalCount;
            LowStockTotalPages = result.TotalPages;
            LowStockHasPreviousPage = result.HasPreviousPage;
            LowStockHasNextPage = result.HasNextPage;
            OnPropertyChanged(nameof(LowStockPaginationInfo));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating low stock report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async Task LowStockGoToPageAsync(int page)
    {
        if (page >= 1 && page <= LowStockTotalPages)
        {
            LowStockCurrentPage = page;
            await GenerateLowStockReportAsync();
        }
    }
    
    private async Task LowStockChangePageSizeAsync()
    {
        LowStockCurrentPage = 1;
        await GenerateLowStockReportAsync();
    }

    private async Task GenerateMovementReportAsync()
    {
        try
        {
            // Use InvariantCulture to ensure consistent date formatting regardless of locale
            var startDateStr = StartDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var endDateStr = EndDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var movements = await _apiClient.GetMovementReportAsync(startDateStr, endDateStr);
            MovementReport = new ObservableCollection<StockMovementDto>(movements.OrderByDescending(m => m.CreatedAt));
            MessageBox.Show($"Movement report generated. {movements.Count} movements found.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating movement report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportStockReport()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = $"StockReport_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = new List<string> { "SKU,Name,Category,Quantity,Price,Low Stock" };
                foreach (var product in StockReport)
                {
                    lines.Add($"{product.SKU},{product.Name},{product.CategoryName},{product.Quantity},{product.Price},{product.IsLowStock}");
                }
                File.WriteAllLines(saveDialog.FileName, lines);
                MessageBox.Show("Report exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportLowStockReport()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = $"LowStockReport_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = new List<string> { "SKU,Name,Category,Quantity,Price,Low Stock Threshold" };
                foreach (var product in LowStockReport)
                {
                    lines.Add($"{product.SKU},{product.Name},{product.CategoryName},{product.Quantity},{product.Price},{product.MinimumStockLevel}");
                }
                File.WriteAllLines(saveDialog.FileName, lines);
                MessageBox.Show("Report exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportMovementReport()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = $"MovementReport_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var lines = new List<string> { "Date,Product,Type,Quantity,Reason,Notes" };
                foreach (var movement in MovementReport)
                {
                    lines.Add($"{movement.CreatedAt:yyyy-MM-dd HH:mm},{movement.ProductName},{movement.MovementTypeName},{movement.Quantity},{movement.Reason ?? ""},{movement.Notes ?? ""}");
                }
                File.WriteAllLines(saveDialog.FileName, lines);
                MessageBox.Show("Report exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
