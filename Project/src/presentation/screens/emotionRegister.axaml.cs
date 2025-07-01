using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Project.application.components;
using System;

namespace Project.presentation.screens
{
    public partial class emotionRegister : Window
    {
        private dayView _day;

        public emotionRegister(dayView day)
        {
            InitializeComponent();
            _day = day;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            _day.EmotionColor = new SolidColorBrush(Colors.Yellow);
            this.Close();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
