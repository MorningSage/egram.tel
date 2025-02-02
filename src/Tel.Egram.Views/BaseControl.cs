using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Tel.Egram.Views;

/// <summary>
/// Workaround for model activation
/// </summary>
public class BaseControl<TViewModel> : ReactiveUserControl<TViewModel> where TViewModel : class
{
    protected BaseControl(bool activate = true)
    {
        if (activate)
        {
            this.WhenActivated(disposables =>
            {
                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }
    }
}