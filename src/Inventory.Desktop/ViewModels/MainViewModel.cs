using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ProductViewModel ProductViewModel { get; }
    public CategoryViewModel CategoryViewModel { get; }
    public StockMovementViewModel StockMovementViewModel { get; }
    public ReportsViewModel ReportsViewModel { get; }
    public UserViewModel? UserViewModel { get; }

    public MainViewModel(ApiClient apiClient, AuthResponseDto? authResponse = null)
    {
        ProductViewModel = new ProductViewModel(apiClient, authResponse);
        CategoryViewModel = new CategoryViewModel(apiClient, authResponse);
        StockMovementViewModel = new StockMovementViewModel(apiClient, authResponse);
        ReportsViewModel = new ReportsViewModel(apiClient, authResponse);

        // Only create UserViewModel if user is SuperAdmin
        if (authResponse != null && authResponse.Role == "SuperAdmin")
        {
            UserViewModel = new UserViewModel(apiClient);
        }
    }
}
