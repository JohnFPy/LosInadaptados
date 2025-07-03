using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;

namespace Project.presentation.screens
{
    public partial class account : UserControl
    {
        private Button? _showUpdateFormButton;
        private Grid? _actualizationGrid;
        private Button? _backButton;
        private Button? _logoutButton;

        public account()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupEventHandlers()
        {
            _showUpdateFormButton = this.FindControl<Button>("ShowUpdateFormButton");
            _actualizationGrid = this.FindControl<Grid>("ActualizationGrid");
            _backButton = this.FindControl<Button>("BackButton");
            _logoutButton = this.FindControl<Button>("LogoutButton");

            if (_showUpdateFormButton != null)
                _showUpdateFormButton.Click += ShowUpdateFormButton_Click;

            if (_backButton != null)
                _backButton.Click += BackButton_Click;

            if (_logoutButton != null)
                _logoutButton.Click += LogoutButton_Click;

            // Establecer visibilidad inicial
            SetInitialVisibility();
        }

        private void SetInitialVisibility()
        {
            if (_showUpdateFormButton != null)
                _showUpdateFormButton.IsVisible = true;

            if (_actualizationGrid != null)
                _actualizationGrid.IsVisible = false;

            if (_backButton != null)
                _backButton.IsVisible = false;
        }

        private void ShowUpdateFormButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_showUpdateFormButton != null)
                _showUpdateFormButton.IsVisible = false;

            if (_actualizationGrid != null)
                _actualizationGrid.IsVisible = true;

            if (_backButton != null)
                _backButton.IsVisible = true;

            if (_logoutButton != null)
                _logoutButton.IsVisible = false;
        }

        private void BackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_showUpdateFormButton != null)
                _showUpdateFormButton.IsVisible = true;

            if (_actualizationGrid != null)
                _actualizationGrid.IsVisible = false;

            if (_backButton != null)
                _backButton.IsVisible = false;

            if (_logoutButton != null)
                _logoutButton.IsVisible = true;
        }

        private void LogoutButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Obtener la aplicación actual
            if (Application.Current is App app)
            {
                // Cambiar el estado de autenticación a false
                app.IsAuthenticated = false;

                // Obtener el ApplicationLifetime
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // Cambiar la vista principal a la vista no autenticada
                    var unauthenticatedView = new UnauthenticatedAreaView();

                    if (desktop.MainWindow != null)
                    {
                        desktop.MainWindow.Content = unauthenticatedView;
                    }
                }
            }
        }
    }
}