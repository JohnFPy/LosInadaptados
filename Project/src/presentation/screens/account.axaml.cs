using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;
using Project.infrastucture;

namespace Project.presentation.screens
{
    public partial class account : UserControl
    {
        private Button? _showUpdateFormButton;
        private Grid? _actualizationGrid;
        private Button? _backButton;
        private Button? _logoutButton;
        private TextBlock? _welcomeTextBlock;

        private int _userId;

        public account()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadUserWelcomeMessage();

            var updateButton = this.FindControl<Button>("ActualizationGrid").FindControl<Button>("Actualizar datos");
            if (updateButton != null)
              //  updateButton.Click += UpdateUserData
              ;
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
            _welcomeTextBlock = this.FindControl<TextBlock>("WelcomeTextBlock");

            if (_showUpdateFormButton != null)
                _showUpdateFormButton.Click += ShowUpdateFormButton_Click;

            if (_backButton != null)
                _backButton.Click += BackButton_Click;

            if (_logoutButton != null)
                _logoutButton.Click += LogoutButton_Click;

            // Establecer visibilidad inicial
            SetInitialVisibility();
        }

        private void LoadUserWelcomeMessage()
        {
            if (_welcomeTextBlock != null && UserSession.IsLoggedIn)
            {
                string userName = UserSession.GetCurrentUserName();
                _welcomeTextBlock.Text = $"Bienvenido, {userName}";
            }
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
            // Limpiar la sesión del usuario
            UserSession.ClearSession();

            // Obtener la aplicación actual
            if (Application.Current is App app)
            {
                // Cambiar el estado de autenticación a false
                app.IsAuthenticated = false;

                // Navegar a la vista no autenticada
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    if (desktop.MainWindow != null)
                    {
                        desktop.MainWindow.Content = new UnauthenticatedAreaView();
                    }
                }
            }
        }
        

        /*private async void UpdateUserData(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newUsername = this.FindControl<TextBox>("NewUsernameTextBox")?.Text;
            var newPassword = this.FindControl<TextBox>("NewPasswordTextBox")?.Text;
            var newName = this.FindControl<TextBox>("NewNameTextBox")?.Text;
            var newLastName = this.FindControl<TextBox>("NewLastNameTextBox")?.Text;
            var newAgeText = this.FindControl<TextBox>("NewAgeTextBox")?.Text;


            if (!string.IsNullOrWhiteSpace(newUsername))
            {
                var result = await UserCRUD.UpdateUsername(_userId, newUsername);
                if (result)
                {
                    // Actualización exitosa del nombre de usuario
                }
                else
                {
                    // Mensaje de error al actualizar el nombre de usuario
                }
            }
        }*/
    }
}