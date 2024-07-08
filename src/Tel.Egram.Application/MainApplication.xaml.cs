using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TdLib;
using Tel.Egram.Services;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Views.Application;
using Tel.Egran.ViewModels.Application;

namespace Tel.Egram.Application;

public class MainApplication : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Registry.Services.GetRequiredService<DatabaseContext>().Database.Migrate();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var model = new MainWindowViewModel();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Startup += (sender, args) => model.Activator.Activate();
            desktop.MainWindow = new MainWindow { DataContext = model };
            desktop.Exit += async (sender, args) =>
            {
                model.Activator.Deactivate();
                await Registry.Services.GetRequiredService<TdClient>().DestroyAsync();
            };
        }
            
        base.OnFrameworkInitializationCompleted();
    }
    
}