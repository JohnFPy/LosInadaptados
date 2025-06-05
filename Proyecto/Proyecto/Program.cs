using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Proyecto
{
    class Program
    {
        // Este método es necesario para el inicio de la aplicación Avalonia
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Esta configuración crea la aplicación Avalonia
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Creamos una ventana con el texto "Hello World"
                desktop.MainWindow = new Window
                {
                    Title = "Mi Aplicación Avalonia",
                    Width = 400,
                    Height = 300,
                    Content = new TextBlock
                    {
                        Text = "¡Hola Mundo!",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        FontSize = 24
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}