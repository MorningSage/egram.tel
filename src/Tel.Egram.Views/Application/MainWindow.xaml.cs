using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Tel.Egran.ViewModels.Application;

namespace Tel.Egram.Views.Application;

public partial class MainWindow : BaseWindow<MainWindowViewModel>
{
    public MainWindow() : base(false)
    {
        this.WhenActivated(disposables =>
        {
            this.BindNotifications()
                .DisposeWith(disposables);
        });
            
        AvaloniaXamlLoader.Load(this);
        this.AttachDevTools();
    }
}