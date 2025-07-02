using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Project.presentation.screens
{
    public partial class canvas : Window
    {
        public canvas()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var vm = DataContext as application.components.canvasView;
            var point = e.GetPosition(this);
            vm?.StartDrawing(point);
        }

        private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            var vm = DataContext as application.components.canvasView;
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var point = e.GetPosition(this);
                vm?.ContinueDrawing(point);
            }
        }
    }
}
