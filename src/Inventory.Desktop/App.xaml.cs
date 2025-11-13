using System.ComponentModel;
using System.Windows;
using Inventory.Desktop.Converters;
using Inventory.Desktop.Services;

namespace Inventory.Desktop
{
    public partial class App : Application, INotifyPropertyChanged
    {
        private const string ApiUrl = "https://localhost:7210/api";
        public static LocalizationService LocalizationService { get; private set; } = null!;
        public static TranslationService TranslationService { get; private set; } = null!;
        
        private string _currentCulture = "en-US";
        
        /// <summary>
        /// Property that changes when culture changes, used for bindings
        /// </summary>
        public string CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize localization services
            LocalizationService = new LocalizationService();
            TranslationService = new TranslationService(LocalizationService.CurrentCulture);
            
            // Set initial culture property
            CurrentCulture = LocalizationService.CurrentCulture.Name;
            
            // Initialize translation extension
            TranslationExtension.Initialize(TranslationService, LocalizationService);

            // Set shutdown mode to only shutdown when main window closes
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            try
            {
                // Show login window first
                var loginWindow = new Views.LoginWindow();
                loginWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when culture changes to update bindings
        /// </summary>
        public void OnCultureChanged()
        {
            CurrentCulture = LocalizationService.CurrentCulture.Name;
        }
    }
}
