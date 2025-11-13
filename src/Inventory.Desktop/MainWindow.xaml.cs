using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;
using Inventory.Desktop.ViewModels;
using System.Windows;

namespace Inventory.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly AuthResponseDto? _authResponse;
        private readonly ApiClient _apiClient;

        public MainWindow(string apiUrl, AuthResponseDto? authResponse = null)
        {
            try
            {
                InitializeComponent();
                
                _authResponse = authResponse;
                
                // Create API client with authentication token
                _apiClient = new ApiClient(apiUrl, authResponse?.Token);
                
                _viewModel = new MainViewModel(_apiClient, authResponse);
                DataContext = _viewModel;
                
                // Display user info
                if (_authResponse != null)
                {
                    UserInfoTextBlock.Text = $"{_authResponse.Username} ({_authResponse.Role})";
                }
                
                // Hide Users tab and menu item if not SuperAdmin
                if (_viewModel.UserViewModel == null)
                {
                    UsersTab.Visibility = Visibility.Collapsed;
                    UsersMenuItem.Visibility = Visibility.Collapsed;
                }

                // Hide Reports tab and menu item if not Moderator or SuperAdmin
                if (!_viewModel.ReportsViewModel.IsModerator)
                {
                    ReportsTab.Visibility = Visibility.Collapsed;
                    ReportsMenuItem.Visibility = Visibility.Collapsed;
                }

                // Ensure the first tab (Products) is selected by default
                MainTabControl.SelectedIndex = 0;

                // Initialize language combo box
                var currentCulture = App.LocalizationService.CurrentCulture.Name;
                foreach (System.Windows.Controls.ComboBoxItem item in LanguageComboBox.Items)
                {
                    if (item.Tag?.ToString() == currentCulture)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing main window: {ex.Message}\n\n{ex.StackTrace}",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw; // Re-throw to be caught by App.xaml.cs
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProductsTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = ProductsTab;
        }

        private void CategoriesTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = CategoriesTab;
        }

        private void StockMovementsTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = StockMovementsTab;
        }

        private void ReportsTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = ReportsTab;
        }

        private void UsersTab_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = UsersTab;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var confirmMessage = App.TranslationService?.Translate("logout.confirm") ?? "Are you sure you want to logout?";
            var logoutTitle = App.TranslationService?.Translate("nav.logout") ?? "Logout";
            var yesText = App.TranslationService?.Translate("common.yes") ?? "Yes";
            var noText = App.TranslationService?.Translate("common.no") ?? "No";
            
            // Create custom message box with translated buttons
            var result = MessageBox.Show(confirmMessage, logoutTitle,
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var oldWindow = this;
            oldWindow.Hide();

            var loginWindow = new Views.LoginWindow();
            if (loginWindow.ShowDialog() == true && loginWindow.AuthResponse != null)
            {
                // Temporarily prevent automatic shutdown
                var previousMode = Application.Current.ShutdownMode;

                // Now safe to close old window
                try
                {
                    oldWindow.Close();
                }
                catch
                {
                    oldWindow.Hide();
                }
                finally
                {
                    // Restore normal shutdown mode
                    Application.Current.ShutdownMode = previousMode;
                }
            }
            else
            {
                oldWindow.Close();
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                var cultureName = selectedItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(cultureName))
                {
                    App.LocalizationService.SetCulture(cultureName);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _apiClient?.Dispose();
            base.OnClosed(e);
        }
    }
}
