using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Project.presentation.Views.AuthViews
{
    public partial class AuthenticatedAreaView : UserControl
    {
        public AuthenticatedAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}