using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using System;

namespace Project.application.components
{
    public class dayTemplateSelector : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data is emptyDayView)
            {
                return new Border
                {
                    Background = Avalonia.Media.Brushes.Transparent,
                    Height = 40,
                    Margin = new Thickness(2),
                    CornerRadius = new CornerRadius(4)
                };
            }

            if (data is dayView day)
            {
                var button = new Button
                {
                    Background = Avalonia.Media.Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Command = day.ClickCommand,
                    Content = new TextBlock
                    {
                        Text = day.DayNumber,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }
                };

                var border = new Border
                {
                    Margin = new Thickness(2),
                    CornerRadius = new CornerRadius(4),
                    Height = 40,
                    Child = button
                };

                // Bind background
                border.Bind(Border.BackgroundProperty, new Binding("EmotionColor"));

                return border;
            }

            throw new NotSupportedException("Unsupported data type");
        }

        public bool Match(object? data)
        {
            return data is dayView;
        }
    }
}
