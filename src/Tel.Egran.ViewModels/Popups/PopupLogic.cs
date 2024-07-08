using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Services;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Popups;

public static class PopupLogic
{
    private static readonly IPopupController PopupController = Registry.Services.GetRequiredService<IPopupController>();
    
    public static IDisposable BindPopup(this PopupModel model)
    {
        return model.Context.CloseCommand
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(_ => PopupController.Hide());
    }
}