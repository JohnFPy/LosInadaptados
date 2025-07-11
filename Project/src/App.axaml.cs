using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;
using Semi.Avalonia;
using System;
using System.IO;
using System.Reflection;

namespace Project;

public partial class App : Application
{
    public bool IsAuthenticated { get; set; } = false; // Propiedad pública

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Apply Semi.Avalonia theme
        this.Styles.Add(new SemiTheme());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var view = IsAuthenticated
                ? (Control)new AuthenticatedAreaView()
                : new UnauthenticatedAreaView();

            desktop.MainWindow = new Window
            {
                Content = view,
                Width = 1280,
                Height = 720,
                Title = "MoodPress",
                WindowState = WindowState.Maximized,
                Icon = LoadWindowIcon() // Cargar el icono de la ventana
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private WindowIcon? LoadWindowIcon()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Buscar el recurso logo.png embebido
            string? logoResourceName = null;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith("logo.png", StringComparison.OrdinalIgnoreCase))
                {
                    logoResourceName = resourceName;
                    break;
                }
            }

            if (logoResourceName != null)
            {
                using var stream = assembly.GetManifestResourceStream(logoResourceName);
                if (stream != null)
                {
                    var bitmap = new Bitmap(stream);
                    return new WindowIcon(bitmap);
                }
            }

            // Si no encuentra el recurso embebido, intentar cargar desde archivo
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "icons", "logo.ico");
            if (File.Exists(iconPath))
            {
                return new WindowIcon(iconPath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading window icon: {ex.Message}");
        }

        return null;
    }
}