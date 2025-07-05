using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Project.application.components;
using System.Diagnostics;

namespace Project.presentation.components
{
    public partial class customEmotionSuccess : Window
    {
        public customEmotionSuccess(string filePath)
        {
            InitializeComponent();
            DataContext = new SuccessViewModel(filePath, this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}