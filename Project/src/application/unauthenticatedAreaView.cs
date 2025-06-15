using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

        public UnauthenticatedAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _registerGrid = this.FindControl<Grid>("RegisterGrid");
            _loginGrid = this.FindControl<Grid>("LoginGrid");
            _ageTextBox = this.FindControl<TextBox>("AgeTextBox");
            _nameTextBox = this.FindControl<TextBox>("NameTextBox");
            _lastNameTextBox = this.FindControl<TextBox>("LastNameTextBox");
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
            var edadTexto = _ageTextBox?.Text;
            var nombreTexto = _nameTextBox?.Text;
            var apellidoTexto = _lastNameTextBox?.Text;

            bool edadValida = RegisterAutentification.EsEdadValida(edadTexto);
            bool nombreValido = RegisterAutentification.EsNombreValido(nombreTexto);
            bool apellidoValido = RegisterAutentification.EsApellidoValido(apellidoTexto); 

            if (edadValida && nombreValido && apellidoValido)
            {
                // Ambos válidos: continuar con el registro
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    window.Content = new AuthenticatedAreaView();
                }
            }
            else
            {
                // Mostrar mensajes de error según corresponda
                if (!edadValida)
                    _ageTextBox!.Watermark = "Introduce una edad válida";
                if (!nombreValido)
                    _nameTextBox!.Watermark = "Nombre inválido (sin números ni espacios)";
                if (!apellidoValido)
                    _lastNameTextBox!.Watermark = "Apellido inválido (sin números ni espacios)";
            }
        }

        private void ValidarNombre()
        {
            var nombre = _nameTextBox?.Text;
            if (!RegisterAutentification.EsNombreValido(nombre))
            {
                _nameTextBox!.Watermark = "Nombre inválido (sin números ni espacios)";
            }
            else
            {
                // Nombre válido, puedes continuar con el flujo
            }
        }
    }
}
