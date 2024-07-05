using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tel.Egram.Model.Application;
using Tel.Egram.Views.Application;

namespace Tel.Egram.Application;

public class MainApplication : Avalonia.Application
{
    public event EventHandler Initializing = delegate { };
    public event EventHandler Disposing= delegate { };
        
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
            
        Initializing?.Invoke(this, EventArgs.Empty);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var model = new MainWindowModel();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Startup += (sender, args) => model.Activator.Activate();
            desktop.MainWindow = new MainWindow { DataContext = model };
            desktop.Exit += (sender, args) =>
            {
                model.Activator.Deactivate();
                Disposing?.Invoke(sender, args);
            };
        }
            
        base.OnFrameworkInitializationCompleted();
    }
        
}