using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;

namespace Project.presentation.components
{
    public partial class audioStatistics : UserControl
    {
        public audioStatistics()
        {
            InitializeComponent();

            // Crear una instancia del ViewModel y asignarla como DataContext
            var viewModel = new Project.application.components.audioStatistics();
            this.DataContext = viewModel;

            // El ViewModel se encargará de crear las cards
            var statisticsContainer = this.FindControl<StackPanel>("StatisticsContainer");
            if (statisticsContainer != null)
            {
                viewModel.CreateStatisticsCards(statisticsContainer);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}