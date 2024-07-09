using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;

namespace Tel.Egram.Model.Settings.Proxy;

public partial class ProxyModel : ObservableObject
{
    [ObservableProperty] private int _id = 0;
    [ObservableProperty] private TdApi.Proxy? _proxy = null;
    [ObservableProperty] private bool _isEnabled = false;
    [ObservableProperty] private bool _isSaved = false;
    [ObservableProperty] private bool _isEditable = false;
    [ObservableProperty] private bool _isRemovable = false;
    [ObservableProperty] private ProxyType _proxyType = ProxyType.Unknown;
    [ObservableProperty] private bool _isServerInputVisible = false;
    [ObservableProperty] private bool _isUsernameInputVisible = false;
    [ObservableProperty] private bool _isPasswordInputVisible = false;
    [ObservableProperty] private bool _isSecretInputVisible = false;
    [ObservableProperty] private string _label = string.Empty;
    [ObservableProperty] private string _unsavedLabel = string.Empty;
    [ObservableProperty] private string _server = string.Empty;
    [ObservableProperty] private string _port = string.Empty;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _secret = string.Empty;
    
    public static ProxyModel DisabledProxy()
    {
        return new ProxyModel
        {
            Id = -1,
            Label = "Disabled proxy",
            IsSaved = true,
            IsServerInputVisible = true,
            IsUsernameInputVisible = true,
            IsPasswordInputVisible = true,
            IsEditable = false,
            IsRemovable = false,
            Server = "",
            Port = "",
            Username = "",
            Password = "",
            Secret = ""
        };
    }
}