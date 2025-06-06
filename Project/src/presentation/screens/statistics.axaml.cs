using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.screens
{
    public partial class statistics : UserControl
    {
        public statistics()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}