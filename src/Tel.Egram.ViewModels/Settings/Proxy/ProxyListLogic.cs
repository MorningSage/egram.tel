using System.Reactive.Disposables;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Settings.Proxy;
using Tel.Egram.Services.Mappers.Proxy;
using Tel.Egram.Services.Settings;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Settings.Proxy;

public static class ProxyListLogic
{
    //public static IDisposable BindProxyLogic(this ProxyPopupContext context, IProxyManager proxyManager, IProxyMapper proxyMapper) => new CompositeDisposable(
    //    context.BindRemoveAction(proxyManager),
    //    context.BindEnableAction(proxyManager),
    //    context.BindAddAction(),
    //    context.BindSaveAction(proxyManager)
    //);
    //
    //private static IDisposable BindRemoveAction(this ProxyPopupContext context, IProxyManager proxyManager)
    //{
    //    context.RemoveProxyCommand = ReactiveCommand.CreateFromObservable(
    //        (ProxyModel proxyModel) => context.RemoveProxy(proxyModel, proxyManager),
    //        null,
    //        RxApp.MainThreadScheduler
    //    );
//
    //    return context.RemoveProxyCommand.Accept(proxyModel =>
    //    {
    //        if (proxyModel == context.SelectedProxy) context.SelectedProxy = context.Proxies.FirstOrDefault();
    //        context.Proxies.Remove(proxyModel);
    //    });
    //}
//
    //private static IDisposable BindEnableAction(this ProxyPopupContext context, IProxyManager proxyManager)
    //{
    //    context.EnableProxyCommand = ReactiveCommand.CreateFromObservable(
    //        (ProxyModel proxyModel) => context.EnableProxy(proxyModel, proxyManager),
    //        null,
    //        RxApp.MainThreadScheduler
    //    );
//
    //    return context.EnableProxyCommand.Accept(proxyModel =>
    //    {
    //        if (proxyModel.IsEnabled) return;
    //        
    //        foreach (var proxy in context.Proxies) proxy.IsEnabled = false;
//
    //        proxyModel.IsEnabled = true;
    //    });
    //}
//
    //private static IDisposable BindAddAction(this ProxyPopupContext context)
    //{
    //    context.AddProxyCommand = ReactiveCommand.CreateFromObservable(
    //        context.AddProxy,
    //        null,
    //        RxApp.MainThreadScheduler
    //    );
//
    //    return context.AddProxyCommand
    //        .Accept(proxyModel =>
    //        {
    //            proxyModel.RemoveCommand = context.RemoveProxyCommand;
    //            proxyModel.EnableCommand = context.EnableProxyCommand;
    //        
    //            context.Proxies.Add(proxyModel);
    //            context.SelectedProxy = proxyModel;
    //        });
    //}
//
    //private static IDisposable BindSaveAction(this ProxyPopupContext context, IProxyManager proxyManager)
    //{   
    //    context.SaveProxyCommand = ReactiveCommand.CreateFromObservable(
    //        (ProxyModel proxyModel) => context.SaveProxy(proxyModel, proxyManager),
    //        null,
    //        RxApp.MainThreadScheduler
    //    );
//
    //    return context.SaveProxyCommand.Accept(proxyModel =>
    //    {
    //        proxyModel.IsSaved = true;
    //    });
    //}
//
    //private static IObservable<ProxyModel> AddProxy(this ProxyPopupContext context)
    //{
    //    var proxyModel = ProxyModel.FromProxy(new TdApi.Proxy
    //    {
    //        Server = null,
    //        Port = 0,
    //        Type = new TdApi.ProxyType.ProxyTypeSocks5
    //        {
    //            Username = null,
    //            Password = null
    //        }
    //    });
    //    
    //    return Observable.Return(proxyModel);
    //}
//
    //private static IObservable<ProxyModel> RemoveProxy(this ProxyPopupContext context, ProxyModel proxyModel, IProxyManager proxyManager)
    //{
    //    if (proxyModel.Proxy != null && proxyModel.Proxy.Id != 0)
    //    {
    //        return proxyManager.RemoveProxy(proxyModel.Proxy).Select(_ => proxyModel);
    //    }
    //        
    //    return Observable.Return(proxyModel);
    //}
//
    //private static IObservable<ProxyModel> SaveProxy(this ProxyPopupContext context, ProxyModel proxyModel, IProxyManager proxyManager)
    //{
    //    if (proxyModel.Proxy?.Id == 0)
    //    {
    //        return proxyManager
    //            .AddProxy(proxyModel.ToProxy())
    //            .Do(proxy => proxyModel.Proxy = proxy)
    //            .Select(_ => proxyModel);
    //    }
//
    //    return proxyManager
    //        .UpdateProxy(proxyModel.Proxy!)
    //        .Do(proxy => proxyModel.Proxy = proxy)
    //        .Select(_ => proxyModel);
    //}
//
    //private static IObservable<ProxyModel> EnableProxy(this ProxyPopupContext context, ProxyModel proxyModel, IProxyManager proxyManager)
    //{
    //    if (proxyModel.IsEnabled) return Observable.Return(proxyModel);
//
    //    return proxyModel.Proxy != null
    //        ? proxyManager.EnableProxy(proxyModel.Proxy).Select(_ => proxyModel)
    //        : proxyManager.DisableProxy().Select(_ => proxyModel);
    //}
}