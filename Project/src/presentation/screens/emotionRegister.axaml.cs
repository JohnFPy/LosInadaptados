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
            DataContext = viewModel;

            // Close the window when event is trigggered
            viewModel.RequestClose += (_, __) => Avalonia.Threading.Dispatcher.UIThread.Post(Close);

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

