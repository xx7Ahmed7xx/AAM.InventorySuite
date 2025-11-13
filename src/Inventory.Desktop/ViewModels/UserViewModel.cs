using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;
using Inventory.Desktop;

namespace Inventory.Desktop.ViewModels;

public class UserViewModel : ViewModelBase
{
    private readonly ApiClient _apiClient;
    private UserDto? _selectedUser;
    private ObservableCollection<UserDto> _users = new();
    
    // Pagination
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalCount = 0;
    private int _totalPages = 0;
    private bool _hasPreviousPage = false;
    private bool _hasNextPage = false;

    public UserViewModel(ApiClient apiClient)
    {
        _apiClient = apiClient;

        LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());
        AddUserCommand = new RelayCommand(_ => AddUser());
        EditUserCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
        DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync(), _ => SelectedUser != null);
        RefreshCommand = new RelayCommand(async _ => await LoadUsersAsync());
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

        _ = LoadUsersAsync();
    }

    public ObservableCollection<UserDto> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public UserDto? SelectedUser
    {
        get => _selectedUser;
        set
        {
            SetProperty(ref _selectedUser, value);
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand LoadUsersCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
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
                    _ = LoadUsersAsync();
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
                return $"Showing {((CurrentPage - 1) * PageSize + 1)} to {Math.Min(CurrentPage * PageSize, TotalCount)} of {TotalCount} users";
            
            var showing = App.TranslationService.Translate("pagination.showing");
            var to = App.TranslationService.Translate("pagination.to");
            var of = App.TranslationService.Translate("pagination.of");
            var users = App.TranslationService.Translate("nav.users");
            return $"{showing} {((CurrentPage - 1) * PageSize + 1)} {to} {Math.Min(CurrentPage * PageSize, TotalCount)} {of} {TotalCount} {users}";
        }
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var result = await _apiClient.GetUsersPagedAsync(CurrentPage, PageSize);
            Users = new ObservableCollection<UserDto>(result.Items);
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasPreviousPage = result.HasPreviousPage;
            HasNextPage = result.HasNextPage;
            OnPropertyChanged(nameof(PaginationInfo));
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("403") || ex.Message.Contains("Forbidden"))
        {
            MessageBox.Show("You do not have permission to access user management. SuperAdmin role required.", 
                "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async Task GoToPageAsync(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await LoadUsersAsync();
        }
    }
    
    private async Task ChangePageSizeAsync()
    {
        CurrentPage = 1;
        await LoadUsersAsync();
    }

    private void AddUser()
    {
        var dialog = new Views.UserDialog(_apiClient);
        if (dialog.ShowDialog() == true)
        {
            CurrentPage = 1;
            _ = LoadUsersAsync();
        }
    }

    private void EditUser()
    {
        if (SelectedUser == null) return;

        var dialog = new Views.UserDialog(_apiClient, SelectedUser);
        if (dialog.ShowDialog() == true)
        {
            CurrentPage = 1;
            _ = LoadUsersAsync();
        }
    }

    private async Task DeleteUserAsync()
    {
        if (SelectedUser == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete user '{SelectedUser.Username}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _apiClient.DeleteUserAsync(SelectedUser.Id);
                CurrentPage = 1;
                await LoadUsersAsync();
                MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
