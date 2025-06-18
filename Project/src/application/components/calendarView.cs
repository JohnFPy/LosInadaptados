using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public calendarView()
        {
            currentDate = DateTime.Today;
            Days = new ObservableCollection<dayView>();
            PreviousMonthCommand = new RelayCommand(_ => PreviousMonth());
            NextMonthCommand = new RelayCommand(_ => NextMonth());

            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            MonthYearText = currentDate.ToString("MMMM yyyy");

            Days.Clear();

            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            DayOfWeek firstDay = new DateTime(currentDate.Year, currentDate.Month, 1).DayOfWeek;
            int offset = ((int)firstDay + 6) % 7; // Inicia en lunes

            for (int i = 0; i < offset; i++)
            {
                Days.Add(new dayView { DayNumber = "", EmotionColor = Brushes.Transparent });
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(new dayView
                {
                    DayNumber = i.ToString(),
                    EmotionColor = (i % 2 == 0) ? Brushes.LightPink : Brushes.LightBlue
                });
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

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Func<object?, bool>? canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => execute(parameter);
    }
}

