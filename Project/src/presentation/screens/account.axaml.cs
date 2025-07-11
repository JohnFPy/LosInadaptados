using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Project.domain;
using Project.infrastucture;
using Project.infrastucture.utils;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;

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
                //updateButton.Click += UpdateUserData;
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

            if (_updateDataButton != null)
                _updateDataButton.Click += UpdateDataButton_Click;

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

        private async void UpdateDataButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            string? newPassword = this.FindControl<TextBox>("NewPasswordTextBox")?.Text;
            string? newAge = this.FindControl<TextBox>("NewAgeTextBox")?.Text;
            string? newName = this.FindControl<TextBox>("NewNameTextBox")?.Text;
            string? newLastname = this.FindControl<TextBox>("NewLastnameTextBox")?.Text;

            bool validPassword = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);
            bool validAge = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);
            bool validName = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);
            bool validLastname = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

            if (!validPassword || !validAge || !validName || !validLastname)
            {
                // Mostrar mensajes de error s
                return;
            }

            var user = UserSession.CurrentUser;
            if (user == null)
                return;

            // Actualizar solo los campos que no son null o vacíos
            if (!string.IsNullOrWhiteSpace(newPassword))
                user.Password = passwordHasher.HashPassword(newPassword);

            if (!string.IsNullOrWhiteSpace(newAge))
                user.Age = int.Parse(newAge);

            if (!string.IsNullOrWhiteSpace(newName))
                user.Name = newName;

            if (!string.IsNullOrWhiteSpace(newLastname))
                user.LastName = newLastname;

            // Actualizar en la base de datos
            var userCrud = new UserCRUD();
            bool result = userCrud.updateUser(user);

            if (result)
            {
                // Actualización exitosa
                if (_welcomeTextBlock != null)
                    _welcomeTextBlock.Text = $"Bienvenido, {user.Username}";
            }
            else
            {
                // Mostrar mensaje de error
            }



            /*if (_newUsernameTextBox == null)
                return;

            string newUsername = _newUsernameTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(newUsername))
                return;

            string currentUsername = UserSession.GetCurrentUserName();
            if (string.IsNullOrWhiteSpace(currentUsername))
                return;

            bool result = await userCrud.UpdateUsername(currentUsername, newUsername);

            if (result)
            {
                if (_welcomeTextBlock != null)
                    _welcomeTextBlock.Text = $"Bienvenido, {newUsername}";

                
                UserSession.CurrentUser.Username = newUsername;
            }
            else
            {
                // Mensaje de error si todo se va a ñonga
            }*/
        }
    }
}