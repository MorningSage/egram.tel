using System.Reactive.Subjects;
using Tel.Egram.Model.Popups;

namespace Tel.Egram.Services.Popups;

public class PopupController : IPopupController
{
    public Subject<PopupContext?> Trigger { get; } = new();
        
    public void Show(PopupContext context)
    {
        Trigger.OnNext(context);
    }

    public void Hide()
    {
        Trigger.OnNext(null);
    }
}