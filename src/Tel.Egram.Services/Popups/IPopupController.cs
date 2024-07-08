using System.Reactive.Subjects;
using Tel.Egram.Model.Popups;

namespace Tel.Egram.Services.Popups;

public interface IPopupController
{
    Subject<PopupContext?> Trigger { get; }
    void Show(PopupContext context);
    void Hide();
}