using Avalonia;
using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Application;

namespace Tel.Egram.Views.Application;

public class MainWindow : WindowWithViewModel<MainWindowViewModel>
{
    public MainWindow()
    {
        //this.WhenActivated(disposables =>
        //{
        //    this.BindNotifications().DisposeWith(disposables);
        //});

        AvaloniaXamlLoader.Load(this);

        this.AttachDevTools();
    }
}