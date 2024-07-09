using TdLib;
using Tel.Egram.Model.Settings.Proxy;

namespace Tel.Egram.Services.Mappers.Proxy;

public interface IProxyMapper
{
    TdApi.Proxy MapToTdApi(ProxyModel proxyModel);
    ProxyModel MapFromTdApi(TdApi.Proxy proxy);
}