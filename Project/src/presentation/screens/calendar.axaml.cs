using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.screens
{
    public partial class calendar : UserControl
    {
        public calendar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}