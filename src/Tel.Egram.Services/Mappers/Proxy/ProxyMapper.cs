using TdLib;
using Tel.Egram.Model.Settings.Proxy;

namespace Tel.Egram.Services.Mappers.Proxy;

public class ProxyMapper : IProxyMapper
{
    public TdApi.Proxy MapToTdApi(ProxyModel proxyModel) => new()
    {
        Id = proxyModel.Proxy?.Id ?? -1,
        IsEnabled = proxyModel.Proxy?.IsEnabled ?? false,
        LastUsedDate = proxyModel.Proxy?.LastUsedDate ?? 0,
        Port = int.TryParse(proxyModel.Port, out var parsedPort) ? parsedPort : 0,
        Server = proxyModel.Server,
        Type = proxyModel.ProxyType switch
        {
            ProxyType.Socks5 => new TdApi.ProxyType.ProxyTypeSocks5
            {
                Username = proxyModel.Username,
                Password = proxyModel.Password
            },
            ProxyType.Http => new TdApi.ProxyType.ProxyTypeHttp
            {
                Username = proxyModel.Username,
                Password = proxyModel.Password
            },
            ProxyType.MtProto => new TdApi.ProxyType.ProxyTypeMtproto
            {
                Secret = proxyModel.Secret
            },
            _ => null
        }
    };
    
    public ProxyModel MapFromTdApi(TdApi.Proxy proxy)
    {
        var model = new ProxyModel
        {
            Id = proxy.Id,
            Proxy = proxy,
            IsEnabled = proxy.IsEnabled,
            IsSaved = proxy.Id != 0,
            Server = proxy.Server ?? "",
            Port = proxy.Port == 0 ? "" : proxy.Port.ToString(),
            Username = "",
            Password = "",
            Secret = ""
        };

        switch (proxy.Type)
        {
            case TdApi.ProxyType.ProxyTypeSocks5 socks5:
                model.ProxyType = ProxyType.Socks5;
                model.Username = socks5.Username ?? "";
                model.Password = socks5.Password ?? "";
                break;
                
            case TdApi.ProxyType.ProxyTypeHttp http:
                model.ProxyType = ProxyType.Http;
                model.Username = http.Username ?? "";
                model.Password = http.Password ?? "";
                break;
                
            case TdApi.ProxyType.ProxyTypeMtproto mtProto:
                model.ProxyType = ProxyType.MtProto;
                model.Secret = mtProto.Secret ?? "";
                break;
        }

        model.IsServerInputVisible = true;
        model.IsUsernameInputVisible = model.ProxyType is ProxyType.Socks5 or ProxyType.Http;
        model.IsPasswordInputVisible = model.ProxyType is ProxyType.Socks5 or ProxyType.Http;
        model.IsSecretInputVisible = model.ProxyType is ProxyType.MtProto;

        model.IsRemovable = true;
        model.IsEditable = true;
            
        return model;
    }
}