using Avalonia.Controls;
using Tel.Egran.ViewModels;

namespace Tel.Egram.Views;

public class WindowWithViewModel<TViewModel> : Window where TViewModel : AbstractViewModelBase
{
    protected WindowWithViewModel()
    {
        // ToDo: Eventually
        //if (activate)
        //{
        //    this.WhenActivated(disposables =>
        //    {
        //        Disposable.Create(() => { }).DisposeWith(disposables);
        //    });
        //}
    }
}