using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using Tel.Egram.Application;

namespace Tel.Egram;

internal abstract class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void CollectServices(IMutableDependencyResolver services)
    {

    }
        
    private static AppBuilder BuildAvaloniaApp()
    {
        // As BuildAvaloniaApp is used by the designer, we collect services here so that things will be loaded correctly
        CollectServices(Locator.CurrentMutable);
        
        return AppBuilder.Configure<MainApplication>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
    }
}