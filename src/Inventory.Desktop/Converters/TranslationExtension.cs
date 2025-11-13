using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Inventory.Desktop.Services;

namespace Inventory.Desktop.Converters;

/// <summary>
/// Markup extension for translating strings in XAML
/// Returns a binding that updates when culture changes
/// </summary>
public class TranslationExtension : MarkupExtension
{
    private static TranslationService? _translationService;
    private static LocalizationService? _localizationService;

    public string Key { get; set; } = string.Empty;

    public TranslationExtension()
    {
    }

    public TranslationExtension(string key)
    {
        Key = key;
    }

    public static void Initialize(TranslationService translationService, LocalizationService localizationService)
    {
        _translationService = translationService;
        _localizationService = localizationService;
        
        // Subscribe to culture changes to trigger UI updates
        _localizationService.CultureChanged += (sender, e) =>
        {
            // Update translation service culture
            _translationService?.SetCulture(_localizationService.CurrentCulture);
            
            // Notify App that culture changed so bindings update
            if (Application.Current is App app)
            {
                app.OnCultureChanged();
            }
        };
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (_translationService == null || _localizationService == null)
        {
            // Fallback if not initialized
            return Key;
        }

        // Return a binding that binds to App.CurrentCulture property
        // This will update when culture changes
        var binding = new Binding(nameof(App.CurrentCulture))
        {
            Source = Application.Current,
            Converter = new TranslationConverter(),
            ConverterParameter = Key,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}

/// <summary>
/// Converter that translates a key based on current culture
/// </summary>
public class TranslationConverter : System.Windows.Data.IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (parameter is string key && App.TranslationService != null)
        {
            return App.TranslationService.Translate(key);
        }
        return parameter?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

