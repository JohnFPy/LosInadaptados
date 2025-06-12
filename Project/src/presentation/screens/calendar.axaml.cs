using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;

namespace Project.presentation.screens
{
    public partial class calendar : UserControl
    {
        public ObservableCollection<DayViewModel> Days { get; set; }
        public string MonthYearText { get; set; }

        public calendar()
        {
            Days = new ObservableCollection<DayViewModel>();
            DateTime today = DateTime.Today;
            MonthYearText = today.ToString("MMMM yyyy");

            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(new DayViewModel
                {
                    DayNumber = i.ToString(),
                    EmotionColor = (i % 2 == 0) ? Brushes.LightPink : Brushes.LightBlue
                });
            }

            InitializeComponent();
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class DayViewModel
    {
        public string DayNumber { get; set; }
        public IBrush EmotionColor { get; set; }
    }
}
