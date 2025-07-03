using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

namespace Project.application.components
{
    public partial class canvas : Window
    {
        public canvas()
        {
            InitializeComponent();
            DataContext = new canvasView();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void RedrawLines()
        {
            var vm = DataContext as canvasView;
            var canvas = this.FindControl<Canvas>("DrawingCanvas");
            canvas.Children.Clear();

            foreach (var line in vm.Lines)
            {
                var path = new Path
                {
                    Stroke = line.Stroke,
                    StrokeThickness = line.Thickness,
                    Data = new LineGeometry(line.Start, line.End)
                };

                // Relative positioning
                Canvas.SetLeft(path, 0);
                Canvas.SetTop(path, 0);
                canvas.Children.Add(path);
            }
        }

        private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var vm = DataContext as canvasView;
            if (sender is Canvas canvasControl)
            {
                var point = e.GetPosition(canvasControl);
                vm?.StartDrawing(point);
                RedrawLines();
            }
        }

        private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            var vm = DataContext as canvasView;
            if (sender is Canvas canvasControl && e.GetCurrentPoint(canvasControl).Properties.IsLeftButtonPressed)
            {
                var point = e.GetPosition(canvasControl);
                vm?.ContinueDrawing(point);
                RedrawLines();
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
