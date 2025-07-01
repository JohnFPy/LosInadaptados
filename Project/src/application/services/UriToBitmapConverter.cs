using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace Project.application.services
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string uriString && Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                try
                {
                    // Avalonia 11.3.2 forma correcta
                    var stream = AssetLoader.Open(uri);
                    return new Bitmap(stream);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
