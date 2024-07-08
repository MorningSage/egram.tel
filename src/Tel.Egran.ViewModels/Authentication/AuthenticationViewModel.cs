using System.Reactive;
using System.Reactive.Disposables;
using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Authentication.Phone;
using Tel.Egram.Model.Authentication.Results;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Settings;
using Tel.Egran.ViewModels.Authentication.Phone;
using Tel.Egran.ViewModels.Authentication.Proxy;

namespace Tel.Egran.ViewModels.Authentication;

[AddINotifyPropertyChangedInterface]
public class AuthenticationViewModel : IActivatableViewModel
{
    public ReactiveCommand<Unit, Unit>? SetProxyCommand { get; set; }
    public ReactiveCommand<AuthenticationViewModel, SendCodeResult>? SendCodeCommand { get; set; }
    public ReactiveCommand<AuthenticationViewModel, CheckCodeResult>? CheckCodeCommand { get; set; }
    public ReactiveCommand<AuthenticationViewModel, CheckPasswordResult>? CheckPasswordCommand { get; set; }

    public int PasswordIndex { get; set; } = 0;
    public int ConfirmIndex { get; set; } = 0;

    public ObservableCollectionExtended<PhoneCodeModel>? PhoneCodes { get; set; }
    public PhoneCodeModel? PhoneCode { get; set; }

    public string PhoneNumber   { get; set; } = string.Empty;
    public int PhoneNumberStart { get; set; } = 0;
    public int PhoneNumberEnd   { get; set; } = 0;

    public string ConfirmCode { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public AuthenticationViewModel(IResourceManager resourceManager, IPopupController popupController, IAuthenticator authenticator, IProxyManager proxyManager)
    {
        this.WhenActivated(disposables =>
        {
            this.LoadPhoneCodes(resourceManager).DisposeWith(disposables);
            this.BindProxyCommand(popupController, proxyManager).DisposeWith(disposables);
            this.BindAuthenticationCommands(authenticator).DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();
}