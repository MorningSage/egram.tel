using ReactiveUI;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Settings;
using Tel.Egran.ViewModels.Settings.Proxy;

namespace Tel.Egran.ViewModels.Authentication.Proxy;

public static class AuthenticationProxyHelper
{
    public static IDisposable BindProxyCommand(this AuthenticationViewModel model, IPopupController popupController, IProxyManager proxyManager) => model.SetProxyCommand = ReactiveCommand.Create(
        () => popupController.Show(new ProxyPopupContext(proxyManager)),
        outputScheduler: RxApp.MainThreadScheduler
    );
}