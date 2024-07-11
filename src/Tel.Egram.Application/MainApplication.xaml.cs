using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TdLib;
using Tel.Egram.Services;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Views.Application;
using Tel.Egram.ViewModels.Application;
using Tel.Egram.ViewModels.DependencyInjection;

namespace Tel.Egram.Application;

public class MainApplication : Avalonia.Application
{
    public IServiceProvider Services { get; } = CollectServices();
    public new static MainApplication Current => (MainApplication) Avalonia.Application.Current!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Services.GetRequiredService<DatabaseContext>().Database.Migrate();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var model = Services.GetRequiredService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = model };
            desktop.Exit += async (sender, args) =>
            {
                await Services.GetRequiredService<TdClient>().DestroyAsync();
            };
        }
            
        base.OnFrameworkInitializationCompleted();
    }
    
    private static ServiceProvider CollectServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.CollectServices().AddViewModels();
        
        return serviceCollection.BuildServiceProvider();
    }
    
}