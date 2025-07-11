using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Project.application.components
{
    public class EmotionToBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                var color = brush.Color;

                if (color.A == 0)
                {
                    return Brushes.Transparent;
                }

                if (parameter?.ToString() == "border")
                {
                    return new SolidColorBrush(color);
                }

                if (parameter?.ToString() == "background")
                {
                    return new SolidColorBrush(Color.FromArgb(60, color.R, color.G, color.B));
                }
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
