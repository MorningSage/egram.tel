using System.Reactive;
using System.Reactive.Disposables;
using DynamicData.Binding;
using ReactiveUI;
using Tel.Egram.Model.Popups;
using Tel.Egram.Model.Settings.Proxy;
using Tel.Egram.Services.Settings;

namespace Tel.Egran.ViewModels.Settings.Proxy;

public class ProxyPopupContext : PopupContext, IActivatableViewModel
{
    public ReactiveCommand<Unit, ProxyModel> AddProxyCommand { get; set; }
        
    public ReactiveCommand<ProxyModel, ProxyModel> SaveProxyCommand { get; set; }
        
    public ReactiveCommand<ProxyModel, ProxyModel> EnableProxyCommand { get; set; }
    public ReactiveCommand<ProxyModel, ProxyModel> RemoveProxyCommand { get; set; }
        
    public bool IsProxyEnabled { get; set; }
        
    public ProxyModel? SelectedProxy { get; set; }
        
    public ObservableCollectionExtended<ProxyModel> Proxies { get; set; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    public ProxyPopupContext(IProxyManager proxyManager)
    {
        this.WhenActivated(disposables =>
        {
            Title = "Proxy configuration";
                
            this.BindProxyLogic(proxyManager)
                .DisposeWith(disposables);
        });
    }
}