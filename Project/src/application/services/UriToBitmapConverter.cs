using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Project.application.components;
using System;
using System.Globalization;
using System.IO;

namespace Project.application.services
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Emotion emotion)
            {
                try
                {
                    if (emotion.IsLocalImage)
                    {
                        // Local
                        if (File.Exists(emotion.ImagePath))
                            return new Bitmap(emotion.ImagePath);
                    }
                    else if (Uri.TryCreate(emotion.ImagePath, UriKind.Absolute, out var uri))
                    {
                        // Compiled
                        var stream = AssetLoader.Open(uri);
                        return new Bitmap(stream);
                    }
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
