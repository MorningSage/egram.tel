﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using Tel.Egram.Application;

namespace Tel.Egram;

public class Program
{
        
    [STAThread]
    public static void Main(string[] args)
    {
        CollectServices();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);

        // Run(Locator.Current);
    }

    public static void CollectServices()
    {
        var resolver = Locator.CurrentMutable;
            
        resolver.AddUtils();
        resolver.AddTdLib();
        resolver.AddPersistance();
        resolver.AddServices();
            
        resolver.AddComponents();
        resolver.AddApplication();
        resolver.AddAuthentication();
        resolver.AddWorkspace();
        resolver.AddSettings();
        resolver.AddMessenger();
    }
        
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<MainApplication>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .UseSkia()
            .LogToTrace();
        
    private static void Run(
        IReadonlyDependencyResolver resolver)
    {
        //var app = resolver.GetService<MainApplication>();
        //var builder = AppBuilder.Configure(app);
        //var runtime = builder.RuntimePlatform.GetRuntimeInfo();
        //var model = new MainWindowModel();
        //
        //switch (runtime.OperatingSystem)
        //{
        //    case OperatingSystemType.OSX:
        //        builder.UseAvaloniaNative()
        //            .With(new AvaloniaNativePlatformOptions
        //            {
        //                UseGpu = true,
        //                UseDeferredRendering = true
        //            })
        //            .UseSkia();
        //        break;
        //    
        //    case OperatingSystemType.Linux:
        //        builder.UseX11()
        //            .With(new X11PlatformOptions
        //            {
        //                UseGpu = true
        //            })
        //            .UseSkia();
        //        break;
        //    
        //    default:
        //        builder.UseWin32()
        //            .With(new Win32PlatformOptions
        //            {
        //                UseDeferredRendering = true
        //            })
        //            .UseSkia();
        //        break;
        //}
//
        //builder.UseReactiveUI();

        //model.Activator.Activate();
        //builder.Start<MainWindow>(() => model);
        //model.Activator.Deactivate();
    }
}