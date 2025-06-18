using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;

namespace Project.presentation.screens
{
    public partial class calendar : UserControl
    {
        public calendar()
        {
            InitializeComponent();
            DataContext = new CalendarView();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
