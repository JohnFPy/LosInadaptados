using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Project.presentation.Views.AuthViews;
using Project.presentation.Views.UnauthViews;
using Semi.Avalonia;

namespace Project;

public partial class App : Application
{
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
            bool isAuthenticated = false; // Cambiar a true para probar la vista autenticada

            var view = isAuthenticated
                ? (Control)new AuthenticatedAreaView()
                : new UnauthenticatedAreaView();

            desktop.MainWindow = new Window
            {
                Content = view,
                Width = 1280,
                Height = 720,
                Title = isAuthenticated ? "Área Autenticada" : "Área No Autenticada"
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}