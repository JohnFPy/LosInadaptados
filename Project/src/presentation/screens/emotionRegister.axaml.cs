using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;

namespace Project.presentation.screens
{
    public partial class emotionRegister : Window
    {
        public emotionRegister(dayView day)
        {
            InitializeComponent();

            // Connect ViewModel
            var viewModel = new emotionRegisterView(day, this);
            this.DataContext = viewModel;

            // Close the eindow when event is trigggered
            viewModel.RequestClose += (_, __) => Avalonia.Threading.Dispatcher.UIThread.Post(Close);

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

