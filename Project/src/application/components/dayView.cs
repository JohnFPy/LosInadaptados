using Avalonia.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Project.application.components
{
    public class dayView : INotifyPropertyChanged
    {
        private string _dayNumber = "";
        public string DayNumber
        {
            get => _dayNumber;
            set
            {
                if (_dayNumber != value)
                {
                    _dayNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private IBrush _emotionColor = Brushes.Transparent;
        public IBrush EmotionColor
        {
            get => _emotionColor;
            set
            {
                if (_emotionColor != value)
                {
                    _emotionColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ClickCommand { get; }

        public dayView()
        {
            ClickCommand = new relayCommand(_ => OnClicked());
        }

        private void OnClicked()
        {
            // EMOTION FORM CONNECTION ###########################
            EmotionColor = Brushes.Yellow;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
