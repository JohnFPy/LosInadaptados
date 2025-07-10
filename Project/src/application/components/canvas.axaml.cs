using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.application.components
{
    public class SavedEmotion
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }


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
                var path = new Avalonia.Controls.Shapes.Path
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

        private void ClearButton_Click(object? sender, RoutedEventArgs e)
        {
            var vm = DataContext as canvasView;
            vm?.Lines.Clear();
            RedrawLines();
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            var nameWindow = new emotionNameInput();
            var name = await nameWindow.ShowDialog<string>(this);

            if (string.IsNullOrWhiteSpace(name))
                return;

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var moodPressPath = Path.Combine(appDataPath, "MoodPress", "customEmotions");
            Directory.CreateDirectory(moodPressPath);

            var uniqueName = $"{name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}.png";
            var filePath = Path.Combine(moodPressPath, uniqueName);



            var renderSurface = this.FindControl<Grid>("RenderSurface");
            var size = renderSurface.Bounds.Size;

            var renderTarget = new RenderTargetBitmap(new PixelSize((int)size.Width, (int)size.Height));
            renderTarget.Render(renderSurface);

            // Saving the image
            using (var stream = File.Create(filePath))
            {
                renderTarget.Save(stream);
            }


            var successWindow = new presentation.components.customEmotionSuccess(filePath);
            await successWindow.ShowDialog(this);


            // Return object with name and path
            var savedEmotion = new SavedEmotion
            {
                Name = name,
                Path = filePath
            };

            this.Close(savedEmotion);
        }

    }
}

