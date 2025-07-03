using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.IO;

namespace Project.application.components
{
    public partial class emotionNameInput : Window
    {
        public string? EmotionName { get; private set; }

        public emotionNameInput()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Accept_Click(object? sender, RoutedEventArgs e)
        {
            EmotionName = this.FindControl<TextBox>("NameTextBox").Text?.Trim();

            if (!string.IsNullOrWhiteSpace(EmotionName))
                this.Close(EmotionName);
        }

        private void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(null);
        }

    }
}
