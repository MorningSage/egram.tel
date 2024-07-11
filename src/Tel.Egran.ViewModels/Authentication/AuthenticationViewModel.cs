using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using TdLib;
using Tel.Egram.Model.Authentication.Phone;
using Tel.Egram.Model.Authentication.Results;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Mappers.Proxy;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Settings;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Settings.Proxy;

namespace Tel.Egran.ViewModels.Authentication;

public partial class AuthenticationViewModel : AbstractViewModelBase
{
    private readonly IPopupController _popupController;
    private readonly IProxyManager _proxyManager;
    private readonly IProxyMapper _proxyMapper;
    private readonly IAuthenticator _authenticator;
    
    [ObservableProperty] private int _passwordIndex;
    [ObservableProperty] private int _confirmIndex;
    [ObservableProperty] private PhoneCodeModel? _phoneCode;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private int _phoneNumberStart;
    [ObservableProperty] private int _phoneNumberEnd;
    [ObservableProperty] private string _confirmCode = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    
    [ObservableProperty] private ObservableCollectionExtended<PhoneCodeModel> _phoneCodes = [];
    
    private bool IsPhoneValid
    {
        get
        {
            var phoneNumber = PhoneNumber;
            var mask        = PhoneCode?.Mask;
            
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
            if (string.IsNullOrWhiteSpace(mask)) return !string.IsNullOrWhiteSpace(phoneNumber);

            return phoneNumber.All(c => char.IsDigit(c) || char.IsWhiteSpace(c)) && phoneNumber.Count(char.IsDigit) == mask.Count(c => !char.IsWhiteSpace(c));
        }
    }

    private bool CanCheckCode => !string.IsNullOrWhiteSpace(ConfirmCode);
    private bool CanCheckPassword => !string.IsNullOrWhiteSpace(Password);

    
    public AuthenticationViewModel(IResourceManager resourceManager, IPopupController popupController, IAuthenticator authenticator, IProxyManager proxyManager, IProxyMapper proxyMapper)
    {
        _popupController = popupController;
        _proxyManager    = proxyManager;
        _proxyMapper     = proxyMapper;
        _authenticator   = authenticator;
        
        Dispatcher.UIThread.Invoke(() =>
        {
            PhoneCodes = new ObservableCollectionExtended<PhoneCodeModel>(resourceManager.GetPhoneCodes().OrderBy(m => m.CountryCode));
            PhoneCode  = PhoneCodes.FirstOrDefault(c => c.CountryCode is "RU", new PhoneCodeModel
            {
                Code        = string.Empty,
                Flag        = null,
                Mask        = string.Empty,
                CountryCode = string.Empty,
                CountryName = string.Empty
            });
        });
        
        _authenticator.ObserveState().SafeSubscribe(state =>
        {
            switch (state)
            {
                case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber _:
                    ConfirmIndex = 0;
                    PasswordIndex = 0;
                    break;
                    
                case TdApi.AuthorizationState.AuthorizationStateWaitCode _:
                    ConfirmIndex = 1;
                    PasswordIndex = 0;
                    break;
                    
                case TdApi.AuthorizationState.AuthorizationStateWaitPassword _:
                    ConfirmIndex = 1;
                    PasswordIndex = 1;
                    break;
            }
        });
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        switch (e.PropertyName)
        {
            case nameof(PhoneNumber):
            case nameof(PhoneCode):
                PhoneNumber = FormatPhone(PhoneNumber, PhoneCode?.Mask ?? string.Empty);
                break;
        }
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

    [RelayCommand]
    private void SetProxy() => _popupController.Show(new ProxyPopupContext(_proxyManager, _proxyMapper));

    [RelayCommand(CanExecute = nameof(IsPhoneValid))]
    private void SendCode()
    {
        var phone  = PhoneCode?.Code + new string(PhoneNumber.Where(char.IsDigit).ToArray());
        var result = _authenticator.SetPhoneNumber(phone).Select(_ => new SendCodeResult());
        // ToDo: Ensure result is success or else display something to user
    }

    [RelayCommand(CanExecute = nameof(CanCheckCode))]
    private void CheckCode()
    {
        var result = _authenticator.CheckCode(ConfirmCode).Select(_ => new CheckCodeResult());
        // ToDo: Ensure result is success or else display something to user
    }

    [RelayCommand(CanExecute = nameof(CanCheckPassword))]
    private void CheckPassword()
    {
        var result = _authenticator.CheckPassword(Password).Select(_ => new CheckPasswordResult());
        // ToDo: Ensure result is success or else display something to user
    }
}