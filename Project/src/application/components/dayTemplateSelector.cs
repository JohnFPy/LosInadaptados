using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using System;

namespace Project.application.components
{
    public class dayTemplateSelector : IDataTemplate
    {
        const int DEFAULT_HEIGHT = 40;
        const int DEFAULT_MARGIN = 2;
        const int DEFAULT_CORNER_RADIUS = 4;

        public Control Build(object? data)
        {

            if (data is emptyDayView)
            {
                return new Border
                {
                    Background = Avalonia.Media.Brushes.Transparent,
                    Height = DEFAULT_HEIGHT,
                    Margin = new Thickness(DEFAULT_MARGIN),
                    CornerRadius = new CornerRadius(DEFAULT_CORNER_RADIUS)
                };
            }

            if (data is dayView day)
            {
                var button = new Button
                {
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Command = day.ClickCommand,
                    Content = new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding("DayNumber"),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }
                };

                var border = new Border
                {
                    Height = DEFAULT_HEIGHT,
                    Margin = new Thickness(DEFAULT_MARGIN),
                    CornerRadius = new CornerRadius(DEFAULT_CORNER_RADIUS),
                    Child = button
                };

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
