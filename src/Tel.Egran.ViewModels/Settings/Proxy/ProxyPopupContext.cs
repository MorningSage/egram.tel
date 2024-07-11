using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using TdLib;
using Tel.Egram.Model.Popups;
using Tel.Egram.Model.Settings.Proxy;
using Tel.Egram.Services.Mappers.Proxy;
using Tel.Egram.Services.Settings;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Settings.Proxy;

/* ToDo: Proxy Support not added yet */

public partial class ProxyPopupContext : PopupContext
{
    private readonly IProxyManager _proxyManager;
    private readonly IProxyMapper _proxyMapper;
    
    [ObservableProperty] private bool _isProxyEnabled;
    [ObservableProperty] private ProxyModel? _selectedProxy;
    [ObservableProperty] private ObservableCollectionExtended<ProxyModel> _proxies;
    
    public ProxyPopupContext(IProxyManager proxyManager, IProxyMapper proxyMapper)
    {
        _proxyManager = proxyManager;
        _proxyMapper = proxyMapper;
        
        Title = "Proxy configuration";

        BindList();
        
        //this.BindProxyLogic(proxyManager, proxyMapper);
    }

    private void BindList()
    {
        /* ToDo: Add event handlers for enabling and removing */
        
        _proxyManager.GetAllProxies().SafeSubscribe(proxies =>
        {
            var disabledProxy = ProxyModel.DisabledProxy();
            //disabledProxy.EnableCommand = context.EnableProxyCommand;
                
            var otherProxies = proxies.Select<TdApi.Proxy, ProxyModel>(p =>
            {
                var proxyModel = _proxyMapper.MapFromTdApi(p);
                //proxyModel.RemoveCommand = RemoveProxyCommand;
                //proxyModel.EnableCommand = EnableProxyCommand;
                return proxyModel;
            }).ToList();
                
            Proxies = [ disabledProxy, ..otherProxies ];
                
            SelectedProxy ??= otherProxies.FirstOrDefault(p => p.IsEnabled) ?? disabledProxy;

            if (proxies.Any(p => p.IsEnabled)) return;
            
            disabledProxy.IsEnabled = true;
        });
    }

    //private void BindEditing()
    //{
    //    var thisEvent  = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h);
    //    var proxyEvent = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h);
    //    
    //    
    //    
    //    
    //    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h)
    //        .Where(pattern => pattern.EventArgs.PropertyName is nameof(SelectedProxy))
    //        .SelectMany(pattern => SelectedProxy is null
    //            ? Observable.Empty<ProxyModel>()
    //            : Observable.Merge(
    //                SelectedProxy.WhenAnyValue(p => p.IsSocks5).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.IsHttp).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.IsMtProto).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.Server).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.Port).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.Username).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.Password).Skip(1).Select(_ => SelectedProxy),
    //                SelectedProxy.WhenAnyValue(p => p.Secret).Skip(1).Select(_ => SelectedProxy)
    //            )
    //        ).Subscribe(obj =>
    //        {
    //            sp.IsSaved = false;
    //                
    //            sp.IsServerInputVisible = true;
    //            sp.IsUsernameInputVisible = sp.IsSocks5 || sp.IsHttp;
    //            sp.IsPasswordInputVisible = sp.IsSocks5 || sp.IsHttp;
    //            sp.IsSecretInputVisible = sp.IsMtProto;
    //        })
    //        ;
    //        
    //    context.WhenAnyValue(c => c.SelectedProxy)
    //        .SelectMany(sp => sp == null
    //            ? Observable.Empty<ProxyModel>()
    //            : Observable.Merge(
    //                sp.WhenAnyValue(p => p.IsSocks5).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.IsHttp).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.IsMtProto).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.Server).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.Port).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.Username).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.Password).Skip(1).Select(_ => sp),
    //                sp.WhenAnyValue(p => p.Secret).Skip(1).Select(_ => sp)))
    //        .Accept(sp =>
    //        {
    //            sp.IsSaved = false;
    //                
    //            sp.IsServerInputVisible = true;
    //            sp.IsUsernameInputVisible = sp.IsSocks5 || sp.IsHttp;
    //            sp.IsPasswordInputVisible = sp.IsSocks5 || sp.IsHttp;
    //            sp.IsSecretInputVisible = sp.IsMtProto;
    //        });
    //}
    
    [RelayCommand] private void Enable(ProxyModel proxyModel) { }
    [RelayCommand] private void Remove(ProxyModel proxyModel) { }
    [RelayCommand] private void AddProxy(ProxyModel proxyModel) { }
    [RelayCommand] private void EnableProxy(ProxyModel proxyModel) { }
    [RelayCommand] private void RemoveProxy(ProxyModel proxyModel) { }
}