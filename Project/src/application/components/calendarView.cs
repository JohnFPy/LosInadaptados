using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;

namespace Project.application.components
{
    public class calendarView
    {
        public ObservableCollection<dayView> Days { get; set; }
        public string MonthYearText { get; set; }

        public calendarView()
        {
            var today = DateTime.Today;
            MonthYearText = today.ToString("MMMM yyyy");

            Days = new ObservableCollection<dayView>();

            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(new dayView
                {
                    DayNumber = i.ToString(),
                    EmotionColor = (i % 2 == 0) ? Brushes.LightPink : Brushes.LightBlue
                });
            }
        }
    }
}
