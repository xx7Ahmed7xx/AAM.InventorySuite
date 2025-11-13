using System.Windows;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.Views;

public partial class CategoryDialog : Window
{
    private readonly ApiClient _apiClient;
    private readonly CategoryDto? _category;

    public CategoryDialog(ApiClient apiClient, CategoryDto? category = null)
    {
        InitializeComponent();
        _apiClient = apiClient;
        _category = category;

        if (_category != null)
        {
            NameTextBox.Text = _category.Name;
            DescriptionTextBox.Text = _category.Description ?? string.Empty;
            Title = App.TranslationService.Translate("categories.edit");
        }
        else
        {
            Title = App.TranslationService.Translate("categories.add");
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (_category != null)
            {
                var updateDto = new CategoryDto
                {
                    Id = _category.Id,
                    Name = NameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim()
                };
                await _apiClient.UpdateCategoryAsync(updateDto);
            }
            else
            {
                var createDto = new CategoryDto
                {
                    Name = NameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim()
                };
                await _apiClient.CreateCategoryAsync(createDto);
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
