using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace Inventory.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly HttpClient _httpClient;
    private readonly string ApiUrl;

    public AuthResponseDto? AuthResponse { get; private set; }

    public LoginWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        UsernameTextBox.Focus();
        
        // Initialize language combo box with saved preference
        var currentCulture = App.LocalizationService?.CurrentCulture.Name ?? "en-US";
        foreach (System.Windows.Controls.ComboBoxItem item in LanguageComboBox.Items)
        {
            if (item.Tag?.ToString() == currentCulture)
            {
                item.IsSelected = true;
                break;
            }
        }
        
        // Set window title
        Title = $"{App.TranslationService.Translate("login.title")} - {App.TranslationService.Translate("login.appTitle")}";
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        ApiUrl = builder.Build().GetSection("ApiUrl").Value ?? "";
    }
    
    private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
        {
            var cultureName = selectedItem.Tag?.ToString();
            if (!string.IsNullOrEmpty(cultureName) && App.LocalizationService != null)
            {
                App.LocalizationService.SetCulture(cultureName);
                // Update translation service
                App.TranslationService?.SetCulture(App.LocalizationService.CurrentCulture);
                // Update window title
                if (App.TranslationService != null)
                {
                    Title = $"{App.TranslationService.Translate("login.title")} - {App.TranslationService.Translate("login.appTitle")}";
                }
            }
        }
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || PasswordBox.Password.Length == 0)
        {
            ShowError("Please enter username and password");
            return;
        }

        // Disable login button to prevent multiple clicks
        var loginButton = sender as System.Windows.Controls.Button;
        if (loginButton != null)
        {
            loginButton.IsEnabled = false;
        }

        try
        {
            var loginDto = new LoginDto
            {
                Username = UsernameTextBox.Text.Trim(),
                Password = PasswordBox.Password
            };

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/auth/login", loginDto);
            
            if (response.IsSuccessStatusCode)
            {
                // Read JSON string and deserialize manually to ensure converter is used
                var jsonString = await response.Content.ReadAsStringAsync();
                // The [JsonConverter] attribute on the Role property will be respected
                AuthResponse = JsonSerializer.Deserialize<AuthResponseDto>(jsonString, JsonOptions.Default);
                
                if (AuthResponse != null)
                {
                    // Launch the main application window
                    LaunchMainApplication();
                    DialogResult = true;
                }
                else
                {
                    ShowError("Failed to parse login response. Please try again.");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ShowError($"Login failed: {response.StatusCode}. {errorContent}");
            }
        }
        catch (HttpRequestException httpEx)
        {
            ShowError($"Cannot connect to API server. Please ensure the API is running at {ApiUrl}\n\nError: {httpEx.Message}");
        }
        catch (TaskCanceledException)
        {
            ShowError("Request timed out. Please check your connection and try again.");
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}\n\nDetails: {ex.GetType().Name}");
        }
        finally
        {
            // Re-enable login button
            if (loginButton != null)
            {
                loginButton.IsEnabled = true;
            }
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string message)
    {
        ErrorTextBlock.Text = message;
        ErrorTextBlock.Visibility = Visibility.Visible;
    }

    private void LaunchMainApplication()
    {
        try
        {
            // Create and show the main window with API URL and auth response
            var mainWindow = new MainWindow(ApiUrl, AuthResponse);
            
            // Set as the application's main window
            Application.Current.MainWindow = mainWindow;
            
            // Show the main window
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Show();
            mainWindow.Activate();
            mainWindow.Focus();

            // Close the login window
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error launching application: {ex.Message}\n\n{ex.StackTrace}",
                "Launch Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            ShowError($"Failed to launch application: {ex.Message}");
        }
    }
}

