using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;

namespace Project.presentation.components
{
    public partial class emotionsStatistics : UserControl
    {
        private readonly emotionsStatisticsViewModel? _viewModel;

        public emotionsStatistics()
        {
            InitializeComponent();

            _viewModel = new emotionsStatisticsViewModel();
            this.DataContext = _viewModel;

            this.Loaded += OnLoaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var container = this.FindControl<StackPanel>("StatisticsContainer");
            if (container != null && _viewModel != null)
            {
                _viewModel.CreateStatisticsCards(container);
            }
        }

        public void RefreshStatistics()
        {
            var container = this.FindControl<StackPanel>("StatisticsContainer");
            if (container != null && _viewModel != null)
            {
                _viewModel.RefreshStatistics(container);
            }
        }
    }
}