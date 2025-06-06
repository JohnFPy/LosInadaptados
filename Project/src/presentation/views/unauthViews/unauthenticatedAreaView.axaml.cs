using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        public UnauthenticatedAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}