using System;
using Avalonia;
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

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<MainApplication>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}