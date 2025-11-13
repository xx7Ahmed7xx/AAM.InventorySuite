using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.ViewModels;

public class CategoryViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private readonly AuthResponseDto? _authResponse;
    private CategoryDto? _selectedCategory;
    private ObservableCollection<CategoryDto> _categories = new();

    public CategoryViewModel(ApiClient apiClient, AuthResponseDto? authResponse = null)
    {
        _apiClient = apiClient;
        _authResponse = authResponse;

        LoadCategoriesCommand = new RelayCommand(async _ => await LoadCategoriesAsync());
        AddCategoryCommand = new RelayCommand(_ => AddCategory());
        EditCategoryCommand = new RelayCommand(_ => EditCategory(), _ => SelectedCategory != null);
        DeleteCategoryCommand = new RelayCommand(async _ => await DeleteCategoryAsync(), _ => SelectedCategory != null);
        RefreshCommand = new RelayCommand(async _ => await LoadCategoriesAsync());

        _ = LoadCategoriesAsync();
    }

    public ObservableCollection<CategoryDto> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public CategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand LoadCategoriesCommand { get; }
    public ICommand AddCategoryCommand { get; }
    public ICommand EditCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Returns true if the current user is Moderator or SuperAdmin
    /// </summary>
    public bool IsModerator => _authResponse != null && 
        (_authResponse.Role == "Moderator" || _authResponse.Role == "SuperAdmin");

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

    private void AddCategory()
    {
        var dialog = new Views.CategoryDialog(_apiClient);
        if (dialog.ShowDialog() == true)
        {
            _ = LoadCategoriesAsync();
        }
    }

    private void EditCategory()
    {
        if (SelectedCategory == null) return;

        var dialog = new Views.CategoryDialog(_apiClient, SelectedCategory);
        if (dialog.ShowDialog() == true)
        {
            _ = LoadCategoriesAsync();
        }
    }

    private async Task DeleteCategoryAsync()
    {
        if (SelectedCategory == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{SelectedCategory.Name}'? This will remove the category from all products.",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.DeleteCategoryAsync(SelectedCategory.Id);
                await LoadCategoriesAsync();
                MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
