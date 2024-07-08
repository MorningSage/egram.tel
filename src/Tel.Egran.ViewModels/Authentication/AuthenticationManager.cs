using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Authentication.Phone;
using Tel.Egram.Model.Authentication.Results;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Authentication;

public static class AuthenticationManager
{
    public static IDisposable BindAuthenticationCommands(this AuthenticationViewModel model, IAuthenticator authenticator)
    {
        var canSendCode      = model.WhenAnyValue(x => x.PhoneNumber).Select(phone => IsPhoneValid(model, phone));
        var canCheckCode     = model.WhenAnyValue(x => x.ConfirmCode).Select(code => !string.IsNullOrWhiteSpace(code));
        var canCheckPassword = model.WhenAnyValue(x => x.Password).Select(password => !string.IsNullOrWhiteSpace(password));

        var disposable = new CompositeDisposable();

        model.WhenAnyValue(m => m.PhoneNumber).ObserveOn(RxApp.MainThreadScheduler).Accept(phone => HandlePhoneChange(model, phone)).DisposeWith(disposable);
        model.WhenAnyValue(m => m.PhoneCode).ObserveOn(RxApp.MainThreadScheduler).Accept(phoneCode => HandlePhoneCodeChange(model, phoneCode)).DisposeWith(disposable);
        
        model.SendCodeCommand = ReactiveCommand.CreateFromObservable(
                (AuthenticationViewModel m) =>
                {
                    var phone = m.PhoneCode.Code + new string(m.PhoneNumber.Where(char.IsDigit).ToArray());
                    return SendCode(phone, authenticator);
                },
                canSendCode,
                RxApp.MainThreadScheduler)
            .DisposeWith(disposable);

        model.CheckCodeCommand = ReactiveCommand.CreateFromObservable(
                (AuthenticationViewModel m) => CheckCode(m.ConfirmCode, authenticator),
                canCheckCode,
                RxApp.MainThreadScheduler)
            .DisposeWith(disposable);
            
        model.CheckPasswordCommand = ReactiveCommand.CreateFromObservable(
                (AuthenticationViewModel m) => CheckPassword(m.Password, authenticator),
                canCheckPassword,
                RxApp.MainThreadScheduler)
            .DisposeWith(disposable);

        authenticator.ObserveState()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(state => HandleState(model, state))
            .DisposeWith(disposable);

        return disposable;
    }

    private static void HandlePhoneChange(AuthenticationViewModel model, string phone)
    {
        // TODO: does not update
        model.PhoneNumber = FormatPhone(phone, model.PhoneCode.Mask);
    }

    private static void HandlePhoneCodeChange(AuthenticationViewModel model, PhoneCodeModel phoneCode)
    {
        model.PhoneNumber = FormatPhone(model.PhoneNumber, phoneCode.Mask);
    }

    private static void HandleState(AuthenticationViewModel model, TdApi.AuthorizationState state)
    {
        switch (state)
        {
            case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber _:
                OnWaitingPhoneNumber(model);
                break;
                    
            case TdApi.AuthorizationState.AuthorizationStateWaitCode _:
                OnWaitingConfirmCode(model);
                break;
                    
            case TdApi.AuthorizationState.AuthorizationStateWaitPassword _:
                OnWaitingPassword(model);
                break;
        }
    }

    private static void OnWaitingPhoneNumber(AuthenticationViewModel model)
    {
        model.ConfirmIndex = 0;
        model.PasswordIndex = 0;
    }

    private static void OnWaitingConfirmCode(AuthenticationViewModel model)
    {
        model.ConfirmIndex = 1;
        model.PasswordIndex = 0;
    }

    private static void OnWaitingPassword(AuthenticationViewModel model)
    {
        model.ConfirmIndex = 1;
        model.PasswordIndex = 1;
    }

    private static IObservable<SendCodeResult> SendCode(string phoneNumber, IAuthenticator authenticator)
    {
        return authenticator.SetPhoneNumber(phoneNumber).Select(_ => new SendCodeResult());
    }

    private static IObservable<CheckCodeResult> CheckCode(string code, IAuthenticator authenticator)
    {
        return authenticator.CheckCode(code).Select(_ => new CheckCodeResult());
    }

    private static IObservable<CheckPasswordResult> CheckPassword(string password, IAuthenticator authenticator)
    {
        return authenticator.CheckPassword(password).Select(_ => new CheckPasswordResult());
    }

    private static bool IsPhoneValid(AuthenticationViewModel model, string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        var mask = model.PhoneCode.Mask;
        
        if (string.IsNullOrWhiteSpace(mask)) return !string.IsNullOrWhiteSpace(phone);

        return phone.All(c => char.IsDigit(c) || char.IsWhiteSpace(c))
            && phone.Count(char.IsDigit) == mask.Count(c => !char.IsWhiteSpace(c));
    }

    private static string FormatPhone(string phone, string mask)
    {
        if (string.IsNullOrWhiteSpace(phone)) return string.Empty;

        var numbers = new string(phone.Where(char.IsDigit).ToArray());
        
        if (string.IsNullOrWhiteSpace(mask))  return numbers;
  
        var builder = new StringBuilder(mask.Length);

        var i = 0;
        foreach (var ch in mask)
        {
            if (i >= numbers.Length) continue;
                
            if (char.IsWhiteSpace(ch))
            {
                builder.Append(' ');
            }
            else
            {
                builder.Append(numbers[i]);
                i++;
            }
        }

        return builder.ToString();
    }
}