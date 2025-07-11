using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Project.presentation.screens;
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
                if (_emotionBrush != value)
                {
                    _emotionBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ClickCommand { get; }

        public string? DateId { get; set; }

        public dayView()
        {
            ClickCommand = new relayCommand(_ => OnClicked());
            EmotionColor.Color = Colors.Transparent;
        }

        private async void OnClicked()
        {
            var window = new emotionRegister(this);
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await window.ShowDialog(desktop.MainWindow);
            }
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
            DateId = null;
        }
    }
}
