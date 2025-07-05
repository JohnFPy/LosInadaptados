using Avalonia;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Project.application.components
{
    public class DrawnLine
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public IBrush Stroke { get; set; } = Brushes.Black;
        public double Thickness { get; set; } = 2;
    }

    public class canvasView : INotifyPropertyChanged
    {
        public class ColorOption
        {
            public string Name { get; set; }
            public IBrush Brush { get; set; }
        }

        private double _lineThickness = 2;
        public double LineThickness
        {
            get => _lineThickness;
            set
            {
                if (_lineThickness != value)
                {
                    _lineThickness = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DrawnLine> Lines { get; set; } = new ObservableCollection<DrawnLine>();
        public ObservableCollection<ColorOption> AvailableColors { get; } = new ObservableCollection<ColorOption>
        {
            new ColorOption { Name = "Negro", Brush = Brushes.Black },
            new ColorOption { Name = "Rojo", Brush = Brushes.Red },
            new ColorOption { Name = "Azul", Brush = Brushes.Blue },
            new ColorOption { Name = "Verde", Brush = Brushes.Green },
            new ColorOption { Name = "Amarillo", Brush = Brushes.Yellow }
        };

        private ColorOption _selectedColor;
        public ColorOption SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();
                }
            }
        }


        private Point? lastPoint = null;

        public void StartDrawing(Point point)
        {
            lastPoint = point;
        }

        public void ContinueDrawing(Point point)
        {
            if (lastPoint != null)
            {
                Lines.Add(new DrawnLine
                {
                    Start = lastPoint.Value,
                    End = point,
                    Stroke = GetBrushFromColor(),
                    Thickness = LineThickness
                });

                lastPoint = point;
            }
        }

        private IBrush GetBrushFromColor()
        {
            return SelectedColor?.Brush ?? Brushes.Black;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
