using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Project.domain; // Importante: importa tu validador
using Project.presentation.Views.AuthViews;
using System;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        private Grid? _registerGrid;
        private Grid? _loginGrid;
        private TextBox? _ageTextBox;

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

            if (RegisterAutentification.EsEdadValida(edadTexto))
            {
                // Edad válida: aquí puedes continuar con el registro
                // Por ejemplo, navegar a la vista autenticada:
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    window.Content = new AuthenticatedAreaView();
                }
            }
            else
            {
                // Edad inválida: muestra un mensaje de error
                // Puedes usar un MessageBox, Snackbar, etc.
                // Ejemplo simple:
                _ageTextBox!.Watermark = "Introduce una edad válida";
            }
        }
    }
}
