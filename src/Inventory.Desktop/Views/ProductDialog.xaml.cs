using System.Windows;
using Inventory.Desktop.DTOs;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.Views
{
    public partial class ProductDialog : Window
    {
        private readonly ApiClient _apiClient;
        private readonly ProductDto? _existingProduct;

        public ProductDialog(ApiClient apiClient, ProductDto? existingProduct = null)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _existingProduct = existingProduct;

            // Load categories and product data
            LoadCategories();
            
            if (existingProduct != null)
            {
                // Hide quantity field when editing (quantity is managed through stock movements)
                QuantityPanel.Visibility = Visibility.Collapsed;
                LoadProductData(existingProduct);
                Title = App.TranslationService.Translate("products.edit");
            }
            else
            {
                Title = App.TranslationService.Translate("products.add");
            }
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _apiClient.GetCategoriesAsync();
                
                // Create a list with a "None" option wrapper
                var categoryList = new List<CategoryDisplayItem>
                {
                    new CategoryDisplayItem { Id = null, DisplayName = "None" }
                };
                
                // Add actual categories
                categoryList.AddRange(categories.Select(c => new CategoryDisplayItem 
                { 
                    Id = c.Id, 
                    DisplayName = c.Name,
                    Category = c
                }));
                
                CategoryComboBox.ItemsSource = categoryList;
                
                // Set selected category after items are loaded
                if (_existingProduct != null)
                {
                    if (_existingProduct.CategoryId.HasValue)
                    {
                        // Find the category display item with matching ID
                        var selectedItem = categoryList.FirstOrDefault(c => c.Id == _existingProduct.CategoryId.Value);
                        CategoryComboBox.SelectedItem = selectedItem;
                    }
                    else
                    {
                        // Select "None" option (first item)
                        CategoryComboBox.SelectedItem = categoryList.First();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Helper class for displaying categories with "None" option
        private class CategoryDisplayItem
        {
            public int? Id { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public CategoryDto? Category { get; set; }
        }

        private void LoadProductData(ProductDto product)
        {
            SkuTextBox.Text = product.SKU;
            NameTextBox.Text = product.Name;
            DescriptionTextBox.Text = product.Description ?? string.Empty;
            BarcodeTextBox.Text = product.Barcode ?? string.Empty;
            PriceTextBox.Text = product.Price.ToString();
            CostTextBox.Text = product.Cost.ToString();
            // Quantity is not editable - it's managed through stock movements
            MinStockTextBox.Text = product.MinimumStockLevel.ToString();
            
            // Category selection is handled in LoadCategories after items are loaded
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                if (_existingProduct == null)
                {
                    // Create new product
                    var createDto = new CreateProductDto
                    {
                        SKU = SkuTextBox.Text.Trim(),
                        Name = NameTextBox.Text.Trim(),
                        Description = DescriptionTextBox.Text.Trim(),
                        Barcode = string.IsNullOrWhiteSpace(BarcodeTextBox.Text) ? null : BarcodeTextBox.Text.Trim(),
                        Price = decimal.Parse(PriceTextBox.Text),
                        Cost = decimal.Parse(CostTextBox.Text),
                        InitialQuantity = int.Parse(QuantityTextBox.Text),
                        MinimumStockLevel = int.Parse(MinStockTextBox.Text),
                        CategoryId = CategoryComboBox.SelectedItem is CategoryDisplayItem item ? item.Id : null
                    };

                    await _apiClient.CreateProductAsync(createDto);
                }
                else
                {
                    // Update existing product
                    var updateDto = new UpdateProductDto
                    {
                        Id = _existingProduct.Id,
                        SKU = SkuTextBox.Text.Trim(),
                        Name = NameTextBox.Text.Trim(),
                        Description = DescriptionTextBox.Text.Trim(),
                        Barcode = string.IsNullOrWhiteSpace(BarcodeTextBox.Text) ? null : BarcodeTextBox.Text.Trim(),
                        Price = decimal.Parse(PriceTextBox.Text),
                        Cost = decimal.Parse(CostTextBox.Text),
                        MinimumStockLevel = int.Parse(MinStockTextBox.Text),
                        CategoryId = CategoryComboBox.SelectedItem is CategoryDisplayItem item ? item.Id : null
                    };

                    await _apiClient.UpdateProductAsync(updateDto);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(SkuTextBox.Text))
            {
                MessageBox.Show("SKU is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out _))
            {
                MessageBox.Show("Price must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(CostTextBox.Text, out _))
            {
                MessageBox.Show("Cost must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            /*if (!int.TryParse(QuantityTextBox.Text, out _))
            {
                MessageBox.Show("Quantity must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }*/

            if (!int.TryParse(MinStockTextBox.Text, out _))
            {
                MessageBox.Show("Minimum stock level must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
