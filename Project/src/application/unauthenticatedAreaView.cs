using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Project.presentation.Views.AuthViews;
//using Project.Domain;
using System;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        private Grid? _mainGrid;
        private Grid? _otherGrid;

        public UnauthenticatedAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _mainGrid = this.FindControl<Grid>("MainGrid");
            _otherGrid = this.FindControl<Grid>("OtherGrid");
        }

        // Método para dirigirse a LoginGrid
        private void ShowOtherGrid(object? sender, RoutedEventArgs e)
        {
            if (_mainGrid == null || _otherGrid == null)
                return;

            _mainGrid.IsVisible = false;
            _otherGrid.IsVisible = true;
        }

        // Método para dirigirse a RegisterGrid
        private void ShowMainGrid(object? sender, RoutedEventArgs e)
        {
            if (_mainGrid == null || _otherGrid == null)
                return;

            _mainGrid.IsVisible = true;
            _otherGrid.IsVisible = false;
        }

        private void OnRegisterClick(object? sender, RoutedEventArgs e)
        {

            var window = this.VisualRoot as Window;
            if (window != null)
            {
                // Crea la vista autenticada y reemplaza el contenido
                var authenticatedView = new AuthenticatedAreaView();
                window.Content = authenticatedView;
            }
        }
    }
}