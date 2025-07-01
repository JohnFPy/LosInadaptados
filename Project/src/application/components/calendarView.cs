using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Project.application.components
{
    public class calendarView : INotifyPropertyChanged
    {
        public ObservableCollection<dayView> Days { get; set; }
        private string _monthYearText;
        public string MonthYearText
        {
            get => _monthYearText;
            set
            {
                if (_monthYearText != value)
                {
                    _monthYearText = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }

        private DateTime currentDate;

        public ICommand TodayCommand { get; }

        public ICommand DayClickCommand { get; }

        public calendarView()
        {
            currentDate = DateTime.Today;
            Days = new ObservableCollection<dayView>();
            PreviousMonthCommand = new relayCommand(_ => PreviousMonth());
            NextMonthCommand = new relayCommand(_ => NextMonth());
            TodayCommand = new relayCommand(_ => Today());

            UpdateCalendar();
        }


        public void Today()
        {
            currentDate = DateTime.Today;
            UpdateCalendar();
        }


        private async void OnDayClicked(dayView clickedDay)
        {
            var window = new emotionRegister(clickedDay);
            await window.ShowDialog((Window)App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        }


        private void UpdateCalendar()
        {
            MonthYearText = currentDate.ToString("MMMM yyyy");

            Days.Clear();

            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            DayOfWeek firstDay = new DateTime(currentDate.Year, currentDate.Month, 1).DayOfWeek;
            int offset = ((int)firstDay + 6) % 7; // Start on Monday

            for (int i = 0; i < offset; i++)
            {
                Days.Add(new emptyDayView());
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                var day = new dayView
                {
                    DayNumber = i.ToString(),
                    EmotionColor = new SolidColorBrush(Colors.Transparent)
                };

                // SUSCRIPCIÓN AL EVENTO
                day.DayClicked += OnDayClicked;

                Days.Add(day);
            }
        }

        public void PreviousMonth()
        {
            currentDate = currentDate.AddMonths(-1);
            UpdateCalendar();
        }

        public void NextMonth()
        {
            currentDate = currentDate.AddMonths(1);
            UpdateCalendar();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

