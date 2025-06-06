using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.screens
{
    public partial class tips : UserControl
    {
        public tips()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}