using Avalonia;
using System;
using System.Diagnostics;
using Project.infrastucture;

namespace Project;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Inicializar base de datos antes de comenzar la aplicación
            Debug.WriteLine("Inicializando la base de datos...");
            DatabaseInitializer.Initialize();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error inicializando base de datos: {ex}");
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
