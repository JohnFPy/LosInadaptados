using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.screens
{
    public partial class account : UserControl
    {
        public account()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}