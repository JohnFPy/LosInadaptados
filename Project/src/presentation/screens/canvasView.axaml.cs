using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Project.presentation.screens
{
    public partial class canvasView : Window
    {
        public canvasView()
        {
            InitializeComponent();
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
