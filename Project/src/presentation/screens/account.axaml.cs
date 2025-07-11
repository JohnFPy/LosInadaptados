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

        private Grid? _actualizationGrid;

        private Button? _showUpdateFormButton;
        private Button? _backButton;
        private Button? _logoutButton;
        private Button? _updateDataButton;

        private TextBox? _newUsernameTextBox;

        private TextBlock? _welcomeTextBlock;

        private int _userId;

        public account()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadUserWelcomeMessage();

            // Corrigir esta línea - ActualizationGrid es un Grid, no un Button
            var actualizationGrid = this.FindControl<Grid>("ActualizationGrid");
            var updateButton = actualizationGrid?.FindControl<Button>("Actualizar datos");
            if (updateButton != null)
            {
                // updateButton.Click += UpdateUserData;
            }
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
            _updateDataButton = this.FindControl<Button>("UpdateDataButton");
            _newUsernameTextBox = this.FindControl<TextBox>("NewUsernameTextBox");

            if (_showUpdateFormButton != null)
                _showUpdateFormButton.Click += ShowUpdateFormButton_Click;

            if (_backButton != null)
                _backButton.Click += BackButton_Click;

            if (_logoutButton != null)
                _logoutButton.Click += LogoutButton_Click;

            //if (_updateDataButton != null)
            //    _updateDataButton.Click += UpdateDataButton_Click;

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

        /*private async void UpdateDataButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_newUsernameTextBox == null)
                return;

            string newUsername = _newUsernameTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                // Mensaje de error si lo deseas
                return;
            }

            // Obtén el ID del usuario actual desde la propiedad CurrentUser
            int userId = UserSession.CurrentUser?.Id ?? 0; // Asegúrate de que 'Id' sea una propiedad válida en el tipo 'user'
            if (userId == 0)
            {
                // Mensaje de error si el ID no es válido
                return;
            }

            var userCrud = new UserCRUD();
            bool result = await userCrud.UpdateUsername(userId, newUsername);

            if (result)
            {
                // Opcional: Actualiza el mensaje de bienvenida
                if (_welcomeTextBlock != null)
                    _welcomeTextBlock.Text = $"Bienvenido, {newUsername}";
                // Opcional: Actualiza la sesión
                UserSession.CurrentUser.Username = newUsername;
            }
            else
            {
                // Mensaje de error
            }
        }*/
    }
}