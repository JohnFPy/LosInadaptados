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
        public ObservableCollection<DrawnLine> Lines { get; set; } = new ObservableCollection<DrawnLine>();

        public ObservableCollection<string> AvailableColors { get; } = new ObservableCollection<string> { "Black", "Red", "Blue", "Green", "Yellow" };

        private string _selectedColor = "Black";
        public string SelectedColor
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

        public ICommand ClearCommand { get; }

        private Point? lastPoint = null;

        public canvasView()
        {
            ClearCommand = new relayCommand(_ =>
            {
                Lines.Clear();
                OnPropertyChanged(nameof(Lines)); // Assuring UI notification
            });
        }

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
                    Stroke = GetBrushFromColor()
                });

                lastPoint = point;
            }
        }

        private IBrush GetBrushFromColor()
        {
            return SelectedColor switch
            {
                "Red" => Brushes.Red,
                "Blue" => Brushes.Blue,
                "Green" => Brushes.Green,
                "Yellow" => Brushes.Yellow,
                _ => Brushes.Black
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
