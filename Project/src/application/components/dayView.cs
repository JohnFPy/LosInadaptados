using Avalonia.Media;
using System;
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

        private SolidColorBrush _emotionBrush = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush EmotionColor
        {
            get => _emotionBrush;
            set
            {
                if (_emotionBrush.Color != value.Color)
                {
                    _emotionBrush.Color = value.Color;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ClickCommand { get; }

        // Event to notify click
        public event Action<dayView>? DayClicked;

        public dayView()
        {
            ClickCommand = new relayCommand(_ => OnClicked());
        }

        private void OnClicked()
        {
            // EMOTION FORM CONNECTION ###########################
            EmotionColor.Color = Colors.Yellow;
            DayClicked?.Invoke(this);

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

namespace Project.application.components
{
    public class emptyDayView : dayView
    {
        public emptyDayView()
        {
            DayNumber = "";
            EmotionColor.Color = Colors.Transparent;
        }
    }
}
