using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Services;
using Tel.Egram.Services.Popups;
using Tel.Egran.ViewModels.Settings.Proxy;

namespace Tel.Egran.ViewModels.Authentication.Proxy;

public static class AuthenticationProxyHelper
{
    private static readonly IPopupController PopupController = Registry.Services.GetRequiredService<IPopupController>();
    
    public static IDisposable BindProxyCommand(this AuthenticationViewModel model) => model.SetProxyCommand = ReactiveCommand.Create(
        () => PopupController.Show(new ProxyPopupContext()),
        outputScheduler: RxApp.MainThreadScheduler
    );
}