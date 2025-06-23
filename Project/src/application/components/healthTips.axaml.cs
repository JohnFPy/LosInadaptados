using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Project.application.components
{
    public partial class HealthTips : UserControl
    {
        private StackPanel _tipsContainer;

        public HealthTips()
        {
            InitializeComponent();
            CreateHealthTips();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _tipsContainer = this.FindControl<StackPanel>("TipsContainer");
        }

        private void CreateHealthTips()
        {
            // Limpiar el contenedor para añadir las tarjetas programáticamente
            _tipsContainer.Children.Clear();

            // Añadir las tarjetas de consejos de salud
            AddHealthTip(
                "Prueba la respiración profunda",
                "Reduce el estrés y mejora tu enfoque");

            AddHealthTip(
                "Haz estiramientos",
                "Mejora tu postura y alivia la tensión");

            AddHealthTip(
                "Mantén una dieta equilibrada",
                "Nutre tu cuerpo con alimentos saludables");
        }

        private void AddHealthTip(string title, string description)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(6),
                Background = new SolidColorBrush(Color.Parse("#1A1A1A")),
                Padding = new Thickness(16, 12),
                Margin = new Thickness(0, 4)
            };

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
            };

            var titleBlock = new TextBlock
            {
                Text = title,
                FontWeight = FontWeight.SemiBold,
                FontSize = 16,
                Foreground = Brushes.White
            };

            var descriptionBlock = new TextBlock
            {
                Text = description,
                Margin = new Thickness(0, 4, 0, 0),
                Opacity = 0.7,
                FontSize = 12,
                Foreground = Brushes.White
            };

            stackPanel.Children.Add(titleBlock);
            stackPanel.Children.Add(descriptionBlock);
            border.Child = stackPanel;

            _tipsContainer.Children.Add(border);
        }
    }
}