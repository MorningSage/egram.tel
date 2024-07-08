using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Popups;
using Tel.Egram.Services.Popups;

namespace Tel.Egran.ViewModels.Popups;

[AddINotifyPropertyChangedInterface]
public class PopupModel : IActivatableViewModel
{
    public PopupContext[] Contexts { get; set; }
    public PopupContext Context { get; set; }

    public bool IsVisible { get; set; } = true;

    public PopupModel(PopupContext context, IPopupController popupController)
    {
        Contexts = [ context ];
        Context = context;
            
        this.WhenActivated(disposables =>
        {
            this.BindPopup(popupController).DisposeWith(disposables);
        });
    }

    private PopupModel()
    {
    }

    public static PopupModel Hidden()
    {
        return new PopupModel
        {
            IsVisible = false
        };
    }

    public ViewModelActivator Activator { get; } = new();
}