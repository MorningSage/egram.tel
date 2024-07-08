using System.Reactive;
using System.Reactive.Disposables;
using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Authentication.Phone;
using Tel.Egram.Model.Authentication.Results;
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

    public AuthenticationViewModel()
    {
        this.WhenActivated(disposables =>
        {
            this.LoadPhoneCodes().DisposeWith(disposables);
            this.BindProxyCommand().DisposeWith(disposables);
            this.BindAuthenticationCommands().DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();
}