using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Project.domain;
using Project.presentation.Views.AuthViews;
using System;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        private Grid? _registerGrid;
        private Grid? _loginGrid;

        private TextBox? _ageTextBox;
        private TextBox? _nameTextBox;
        private TextBox? _lastNameTextBox;
        private TextBox? _passwordTextBox;

        private TextBlock? _nameErrorTextBlock;

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

            _ageTextBox = this.FindControl<TextBox>("AgeTextBox");
            _nameTextBox = this.FindControl<TextBox>("NameTextBox");
            _lastNameTextBox = this.FindControl<TextBox>("LastNameTextBox"); 
            _passwordTextBox = this.FindControl<TextBox>("PasswordTextBox"); 

            // TextBlock para mostrar errores de valores ingresados en el registro

            _nameErrorTextBlock = this.FindControl<TextBlock>("NameErrorTextBlock");
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
            var ageText = _ageTextBox?.Text;
            var nameText = _nameTextBox?.Text;
            var lastNameText = _lastNameTextBox?.Text; 
            var contraseñaTexto = _passwordTextBox?.Text;

            bool validAge = RegisterAutentification.IsValidAge(ageText);
            bool validName = RegisterAutentification.IsValidName(nameText);
            bool validLastName = RegisterAutentification.IsValidLastName(lastNameText);
            bool validPassword = RegisterAutentification.IsValidPassword(contraseñaTexto);

            // Ocultar mensajes de error antes de validar

            _nameErrorTextBlock!.IsVisible = false;

            if (validAge && validName && validLastName && validPassword)
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
                if (!validAge)
                    _ageTextBox!.Watermark = "Introduce una edad válida";

                if (!validName)
                {
                    _nameErrorTextBlock.Text = "Nombre inválido (sin números ni espacios)";
                    _nameErrorTextBlock.IsVisible = true;
                }

                if (!validLastName)
                    _lastNameTextBox!.Watermark = "Apellido inválido (sin números ni espacios)";

                if (!validPassword)
                    _passwordTextBox!.Watermark = "Contraseña inválida (mínimo 8 caracteres, 1 mayúscula, 1 minúscula y 1 número)";
            }
        }
    }
}
