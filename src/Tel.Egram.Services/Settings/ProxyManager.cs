using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Settings;

public class ProxyManager(IAgent agent) : IProxyManager
{
    public IObservable<TdApi.Proxy[]> GetAllProxies()
    {
        return agent.Execute(new TdApi.GetProxies()).Select(p => p.Proxies_);
    }

    public IObservable<TdApi.Proxy> AddProxy(TdApi.Proxy proxy) => agent.Execute(new TdApi.AddProxy
    {
        Server = proxy.Server,
        Port   = proxy.Port,
        Type   = proxy.Type
    });

    public IObservable<TdApi.Proxy> UpdateProxy(TdApi.Proxy proxy) => agent.Execute(new TdApi.EditProxy
    {
        ProxyId = proxy.Id,
        Server  = proxy.Server,
        Port    = proxy.Port,
        Type    = proxy.Type
    });

    public IObservable<TdApi.Ok> RemoveProxy(TdApi.Proxy proxy) => agent.Execute(new TdApi.RemoveProxy
    {
        ProxyId = proxy.Id
    });

    public IObservable<TdApi.Ok> EnableProxy(TdApi.Proxy proxy) => agent.Execute(new TdApi.EnableProxy
    {
        ProxyId = proxy.Id
    });

    public IObservable<TdApi.Ok> DisableProxy() => agent.Execute(new TdApi.DisableProxy());
}