using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Project.infrastucture;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Project.domain;
using Project.presentation.Views.AuthViews;
using System;
using System.Drawing.Printing;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        private Grid? _registerGrid;
        private Grid? _loginGrid;

        private TextBox? _usernameTextBox;
        private TextBox? _passwordTextBox;
        private TextBox? _nameTextBox;
        private TextBox? _lastnameTextBox;
        private TextBox? _ageTextBox;

        
        private TextBlock? _usernameErrorTextBlock;
        private TextBlock? _passwordErrorTextBlock;
        private TextBlock? _nameErrorTextBlock;
        private TextBlock? _lastnameErrorTextBlock;
        private TextBlock? _ageErrorTextBlock;

        public UnauthenticatedAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Mostrar Register y Login grid

            _registerGrid = this.FindControl<Grid>("RegisterGrid");
            _loginGrid = this.FindControl<Grid>("LoginGrid");

            // TextBox de validacion de datos

            _usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
            _passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");
            _nameTextBox = this.FindControl<TextBox>("NameTextBox");
            _lastnameTextBox = this.FindControl<TextBox>("LastnameTextBox");
            _ageTextBox = this.FindControl<TextBox>("AgeTextBox");

            // TextBlock para mostrar errores de valores ingresados en el registro

            _usernameErrorTextBlock = this.FindControl<TextBlock>("UsernameErrorTextBlock");
            _passwordErrorTextBlock = this.FindControl<TextBlock>("PasswordErrorTextBlock");
            _nameErrorTextBlock = this.FindControl<TextBlock>("NameErrorTextBlock");
            _lastnameErrorTextBlock = this.FindControl<TextBlock>("LastnameErrorTextBlock");
            _ageErrorTextBlock = this.FindControl<TextBlock>("AgeErrorTextBlock");
        }

        // Método para dirigirse a LoginGrid
        private void ShowLoginGrid(object? sender, RoutedEventArgs e)
        {
            if (_registerGrid == null || _loginGrid == null)
                return;

            _registerGrid.IsVisible = false;
            _loginGrid.IsVisible = true;
        }

        // Método para dirigirse a RegisterGrid
        private void ShowRegisterGrid(object? sender, RoutedEventArgs e)
        {
            if (_registerGrid == null || _loginGrid == null)
                return;

            _registerGrid.IsVisible = true;
            _loginGrid.IsVisible = false;
        }

        private void OnRegisterClick(object? sender, RoutedEventArgs e)
        {

            var usernameText = _usernameTextBox?.Text;
            var passwordText = _passwordTextBox?.Text;
            var nameText = _nameTextBox?.Text;
            var lastnameText = _lastnameTextBox?.Text; 
            var ageText = _ageTextBox?.Text;

            bool validUsername = RegisterAutentification.IsValidUsername(usernameText);
            bool validPassword = RegisterAutentification.IsValidPassword(passwordText);
            bool validName = RegisterAutentification.IsValidName(nameText);
            bool validLastname = RegisterAutentification.IsValidLastname(lastnameText);
            bool validAge = RegisterAutentification.IsValidAge(ageText);

            // Ocultar mensajes de error antes de validar

            _usernameErrorTextBlock!.IsVisible = false;
            _passwordErrorTextBlock!.IsVisible = false;
            _nameErrorTextBlock!.IsVisible = false;
            _lastnameErrorTextBlock!.IsVisible = false;
            _ageErrorTextBlock!.IsVisible = false;

            // Validar los datos ingresados

            if (validUsername && validPassword && validName && validLastname && validAge)
            {
                // Guardar en la base de datos usando UserCRUD
                var userCrud = new Project.infrastucture.UserCRUD();
                var newUser = new Project.domain.models.user
                {
                    Username = usernameText!,
                    Password = passwordText!, // Considera hashear la contraseña si tu sistema lo requiere
                    Name = nameText!,
                    LastName = lastnameText!,
                    Age = int.Parse(ageText!),
                    PathImage = "" // O asigna un valor por defecto si corresponde
                };

                userCrud.SignUp(newUser);

                // Todos los datos son válidos, proceder a la siguiente vista
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    window.Content = new AuthenticatedAreaView();
                }
            }


            if (validUsername && validPassword && validName && validLastname && validAge)
            {
                // Todos los datos son válidos, proceder a la siguiente vista
                var window = this.VisualRoot as Window;

                if (window != null)
                {
                    window.Content = new AuthenticatedAreaView();
                }
            }
            else
            {
                // Mostrar mensajes de error según corresponda


                if (!validUsername)
                {
                    _usernameErrorTextBlock.Text = "El nombre de usuario no debe contener espacios. Máximo 10 caracteres";
                    _usernameErrorTextBlock.IsVisible = true;
                }
                if (!validPassword)
                { 
                    _passwordErrorTextBlock.Text = "Mínimo 8 caracteres, 1 mayúscula, 1 minúscula y 1 número)";
                    _passwordErrorTextBlock!.IsVisible = true;
                }
                if (!validName)
                {
                    _nameErrorTextBlock.Text = "El nombre no puede tener espacios, números o símbolos";
                    _nameErrorTextBlock.IsVisible = true;
                }

                if (!validLastname)
                { 
                    _lastnameErrorTextBlock.Text = "El apellido no puede tener espacios, números o símbolos";
                    _lastnameErrorTextBlock.IsVisible = true;
                }
                if (!validAge)
                {
                    _ageErrorTextBlock.Text = "La edad debe ser un número entero positivo";
                    _ageErrorTextBlock.IsVisible = true;
                }
            }
        }


        private void OnLoginClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Busca los TextBox del login
            var loginUsernameTextBox = this.FindControl<TextBox>("LoginUsernameTextBox");
            var loginPasswordTextBox = this.FindControl<TextBox>("LoginPasswordTextBox");

            var username = loginUsernameTextBox?.Text;
            var password = loginPasswordTextBox?.Text;

            // Validación 

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {

                _usernameErrorTextBlock!.IsVisible = true;
                _usernameErrorTextBlock.Text = "Nombre de usuario o contraseña incorrectos.";

                return;
            }


            var userCrud = new Project.infrastucture.UserCRUD();
            bool acceso = userCrud.Login(username, password);

            if (acceso)
            {
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    window.Content = new AuthenticatedAreaView();
                }
            }
            else
            {
                // Credenciales inválidas, mostrar mensaje de error
                _usernameErrorTextBlock!.IsVisible = true;
                _usernameErrorTextBlock.Text = "Nombre de usuario o contraseña incorrectos.";
            }
        }
    }
}
