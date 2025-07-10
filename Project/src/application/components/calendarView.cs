using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Project.domain.services;
using Project.application.services;

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
        public ICommand TodayCommand { get; }

        private DateTime currentDate;
        private readonly DatabaseEmotionFetcher emotionFetcher = new();

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
                var date = new DateTime(currentDate.Year, currentDate.Month, i);
                string dateId = date.ToString("yyyy-MM-dd");

                SolidColorBrush color = new SolidColorBrush(Colors.Transparent);

                var emotionLog = emotionFetcher.GetEmotionLogForDate(dateId);
                if (emotionLog.HasValue)
                {
                    string? name = null;
                    bool isPersonalized = false;

                    if (emotionLog.Value.emotionId.HasValue)
                    {
                        name = emotionFetcher.GetEmotionNameById(emotionLog.Value.emotionId.Value);
                    }
                    else if (emotionLog.Value.personalizedEmotionId.HasValue)
                    {
                        name = emotionFetcher.GetPersonalizedEmotionNameById(emotionLog.Value.personalizedEmotionId.Value);
                        isPersonalized = true;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        var hex = emotionColorMapper.GetColor(name, isPersonalized);
                        color = new SolidColorBrush(Color.Parse(hex));
                    }
                }

                Days.Add(new dayView
                {
                    DayNumber = i.ToString(),
                    EmotionColor = color,
                    DateId = dateId
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
