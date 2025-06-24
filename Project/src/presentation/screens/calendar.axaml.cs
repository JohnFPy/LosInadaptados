using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Project.application.components;
using System;
using System.Data;

namespace Project.presentation.screens
{
    public partial class calendar : UserControl
    {
        public calendar()
        {
            InitializeComponent();

            DataContext = new calendarView();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
