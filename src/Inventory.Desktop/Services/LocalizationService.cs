using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace Inventory.Desktop.Services;

/// <summary>
/// Service for managing application localization
/// </summary>
public class LocalizationService
{
    private CultureInfo _currentCulture;
    private const string LocaleConfigFile = "locale.json";

    public event EventHandler? CultureChanged;

    public LocalizationService()
    {
        // Load saved culture from config file or default to English
        string savedCulture = "en-US";
        try
        {
            if (File.Exists(LocaleConfigFile))
            {
                var json = File.ReadAllText(LocaleConfigFile);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (config != null && config.TryGetValue("locale", out var locale))
                {
                    savedCulture = locale;
                }
            }
        }
        catch
        {
            // If reading fails, use default
        }
        
        // Initialize _currentCulture before calling SetCulture
        _currentCulture = new CultureInfo(savedCulture);
        SetCulture(savedCulture);
    }

    public CultureInfo CurrentCulture => _currentCulture;

    public void SetCulture(string cultureName)
    {
        try
        {
            _currentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;

            // Save to config file
            try
            {
                var config = new Dictionary<string, string> { { "locale", cultureName } };
                var json = JsonSerializer.Serialize(config);
                File.WriteAllText(LocaleConfigFile, json);
            }
            catch
            {
                // If saving fails, continue anyway
            }

            // Update FlowDirection for RTL languages
            if (_currentCulture.TextInfo.IsRightToLeft)
            {
                Application.Current.Resources["FlowDirection"] = FlowDirection.RightToLeft;
            }
            else
            {
                Application.Current.Resources["FlowDirection"] = FlowDirection.LeftToRight;
            }

            CultureChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            // Fallback to English if culture is invalid
            System.Diagnostics.Debug.WriteLine($"Failed to set culture {cultureName}: {ex.Message}");
            _currentCulture = new CultureInfo("en-US");
        }
    }

    public string GetString(string key)
    {
        // For now, return the key. In a full implementation, you would load from .resx files
        // This is a placeholder - you would need to create .resx files for each language
        return key;
    }
}

