using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Project.domain;
using Project.infrastucture;
using Project.infrastucture.utils;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;

namespace Project.application.components
{
    public partial class account : UserControl
    {

        private Grid? _actualizationGrid;

        private Button? _showUpdateFormButton;
        private Button? _backButton;
        private Button? _logoutButton;
        private Button? _updateDataButton;
        private TextBlock? _usernameErrorTextBlock;
        private TextBlock? _passwordErrorTextBlock;
        private TextBlock? _nameErrorTextBlock;
        private TextBlock? _lastnameErrorTextBlock;
        private TextBlock? _ageErrorTextBlock;


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

            _usernameErrorTextBlock = this.FindControl<TextBlock>("UsernameErrorTextBlock");
            _passwordErrorTextBlock = this.FindControl<TextBlock>("PasswordErrorTextBlock");
            _nameErrorTextBlock = this.FindControl<TextBlock>("NameErrorTextBlock");
            _lastnameErrorTextBlock = this.FindControl<TextBlock>("LastnameErrorTextBlock");
            _ageErrorTextBlock = this.FindControl<TextBlock>("AgeErrorTextBlock");


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
            ClearAllErrorMessages();

            string? newUsername = this.FindControl<TextBox>("NewUsernameTextBox")?.Text;
            string? newPassword = this.FindControl<TextBox>("NewPasswordTextBox")?.Text;
            string? newAge = this.FindControl<TextBox>("NewAgeTextBox")?.Text;
            string? newName = this.FindControl<TextBox>("NewNameTextBox")?.Text;
            string? newLastname = this.FindControl<TextBox>("NewLastnameTextBox")?.Text;

            bool isUsernameValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);
            bool validPassword = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);
            bool validAge = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);
            bool validName = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);
            bool validLastname = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

            bool hasErrors = false;

            if (!isUsernameValid)
            {
                _usernameErrorTextBlock!.Text = "Nombre de usuario inválido.";
                _usernameErrorTextBlock.IsVisible = true;
                hasErrors = true;
            }

            if (!validPassword)
            {
                _passwordErrorTextBlock!.Text = "Contraseña inválida.";
                _passwordErrorTextBlock.IsVisible = true;
                hasErrors = true;
            }

            if (!validAge)
            {
                _ageErrorTextBlock!.Text = "Edad inválida.";
                _ageErrorTextBlock.IsVisible = true;
                hasErrors = true;
            }

            if (!validName)
            {
                _nameErrorTextBlock!.Text = "Nombre inválido.";
                _nameErrorTextBlock.IsVisible = true;
                hasErrors = true;
            }

            if (!validLastname)
            {
                _lastnameErrorTextBlock!.Text = "Apellido inválido.";
                _lastnameErrorTextBlock.IsVisible = true;
                hasErrors = true;
            }

            if (hasErrors)
                return;

            var user = UserSession.CurrentUser;
            if (user == null)
                return;

            if (!string.IsNullOrWhiteSpace(newUsername) && isUsernameValid)
                user.Username = newUsername;

            if (!string.IsNullOrWhiteSpace(newPassword))
                user.Password = passwordHasher.HashPassword(newPassword);

            if (!string.IsNullOrWhiteSpace(newAge))
                user.Age = int.Parse(newAge);

            if (!string.IsNullOrWhiteSpace(newName))
                user.Name = newName;

            if (!string.IsNullOrWhiteSpace(newLastname))
                user.LastName = newLastname;

            var userCrud = new UserCRUD();
            bool result = userCrud.updateUser(user);

            if (result)
            {
                _welcomeTextBlock!.Text = $"Bienvenido, {user.Username}";
                BackButton_Click(sender, e);
            }
            else
            {
                _usernameErrorTextBlock!.Text = "Error al actualizar los datos.";
                _usernameErrorTextBlock.IsVisible = true;
            }
        }

        private void ClearAllErrorMessages()
        {
            _usernameErrorTextBlock!.IsVisible = false;
            _passwordErrorTextBlock!.IsVisible = false;
            _nameErrorTextBlock!.IsVisible = false;
            _lastnameErrorTextBlock!.IsVisible = false;
            _ageErrorTextBlock!.IsVisible = false;
        }

    }
}