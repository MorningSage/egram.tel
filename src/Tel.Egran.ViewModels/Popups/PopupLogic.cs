using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Popups;

public static class PopupLogic
{
    public static IDisposable BindPopup(this PopupModel model, IPopupController popupController)
    {
        return model.Context.CloseCommand
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(_ => popupController.Hide());
    }
}