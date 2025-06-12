using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

        // Este método se llama al hacer clic en "Ya estás registrado?"
        private void ShowOtherGrid(object? sender, RoutedEventArgs e)
        {
            if (_mainGrid == null || _otherGrid == null)
                return;

            _mainGrid.IsVisible = false;
            _otherGrid.IsVisible = true;
        }

        // Este método se llama al hacer clic en "Volver"
        private void ShowMainGrid(object? sender, RoutedEventArgs e)
        {
            if (_mainGrid == null || _otherGrid == null)
                return;

            _mainGrid.IsVisible = true;
            _otherGrid.IsVisible = false;
        }
    }
}