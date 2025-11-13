using System.Windows;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Enums;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.Views;

public partial class UserDialog : Window
{
    private readonly ApiClient _apiClient;
    private readonly UserDto? _existingUser;
    private readonly bool _isEditMode;

    public UserDialog(ApiClient apiClient, UserDto? existingUser = null)
    {
        InitializeComponent();
        _apiClient = apiClient;
        _existingUser = existingUser;
        _isEditMode = existingUser != null;

        Title = _isEditMode ? App.TranslationService.Translate("users.edit") : App.TranslationService.Translate("users.add");

        if (_isEditMode && _existingUser != null)
        {
            UsernameTextBox.Text = _existingUser.Username;
            EmailTextBox.Text = _existingUser.Email;
            // Set role from string
            if (Enum.TryParse<UserRole>(_existingUser.Role, out var role))
            {
                RoleComboBox.SelectedItem = role;
            }
            IsActiveCheckBox.IsChecked = _existingUser.IsActive;
            PasswordBox.Visibility = Visibility.Collapsed;
            // Hide password label - find by Grid.Row instead of text since it's now translated
            var passwordLabel = ((System.Windows.Controls.Grid)Content).Children
                .OfType<System.Windows.Controls.TextBlock>()
                .FirstOrDefault(tb => System.Windows.Controls.Grid.GetRow(tb) == 4);
            if (passwordLabel != null)
                passwordLabel.Visibility = Visibility.Collapsed;
        }
        else
        {
            RoleComboBox.SelectedItem = UserRole.Cashier;
            IsActiveCheckBox.IsChecked = true;
        }

        UsernameTextBox.Focus();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
        {
            MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!_isEditMode && PasswordBox.Password.Length == 0)
        {
            MessageBox.Show("Password is required for new users.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (_isEditMode && _existingUser != null)
            {
                var updateDto = new UpdateUserDto
                {
                    Id = _existingUser.Id,
                    Username = UsernameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    Password = PasswordBox.Password.Length > 0 ? PasswordBox.Password : null,
                    Role = (RoleComboBox.SelectedItem as UserRole?)?.ToString() ?? "Cashier",
                    IsActive = IsActiveCheckBox.IsChecked ?? true
                };

                await _apiClient.UpdateUserAsync(updateDto);
                DialogResult = true;
                Close();
            }
            else
            {
                var createDto = new CreateUserDto
                {
                    Username = UsernameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    Password = PasswordBox.Password,
                    Role = (RoleComboBox.SelectedItem as UserRole?)?.ToString() ?? "Cashier",
                    IsActive = IsActiveCheckBox.IsChecked ?? true
                };

                await _apiClient.CreateUserAsync(createDto);
                DialogResult = true;
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
