using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Application;
using Tel.Egram.Services;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;
using Tel.Egran.ViewModels.Authentication;
using Tel.Egran.ViewModels.Popups;
using Tel.Egran.ViewModels.Workspace;

namespace Tel.Egran.ViewModels.Application;

[AddINotifyPropertyChangedInterface]
public class MainWindowViewModel : IActivatableViewModel
{
    public StartupModel? StartupModel { get; set; }
        
    public AuthenticationViewModel? AuthenticationModel { get; set; }
        
    public WorkspaceModel? WorkspaceModel { get; set; }
        
    public ViewModelActivator Activator { get; } = new();
    
    public PopupModel? PopupModel { get; set; }

    public int PageIndex { get; set; } = 0;

    public string WindowTitle { get; set; } = string.Empty;

    public string ConnectionState { get; set; } = "Initializing...";

    public MainWindowViewModel()
    {
        this.WhenActivated(disposables =>
        {
            BindAuthentication().DisposeWith(disposables);
            BindConnectionInfo().DisposeWith(disposables);
            BindPopup().DisposeWith(disposables);
        });
    }

    private IDisposable BindPopup()
    {
        PopupModel = PopupModel.Hidden();

        return Registry.Services.GetRequiredService<IPopupController>().Trigger
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(context => PopupModel = context == null ? PopupModel.Hidden() : new PopupModel(context));
    }
    
    private IDisposable BindConnectionInfo()
    {
        return Registry.Services.GetRequiredService<IAgent>().Updates.OfType<TdApi.Update.UpdateConnectionState>()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(update => ConnectionState = update.State switch
            {
                TdApi.ConnectionState.ConnectionStateConnecting        => "Connecting...",
                TdApi.ConnectionState.ConnectionStateConnectingToProxy => "Connecting to proxy...",
                TdApi.ConnectionState.ConnectionStateReady             => "Ready.",
                TdApi.ConnectionState.ConnectionStateUpdating          => "Updating...",
                TdApi.ConnectionState.ConnectionStateWaitingForNetwork => "Waiting for network...",
                _                                                      => update.State.GetType().Name
            });
    }
    
    private CompositeDisposable BindAuthentication()
    {
        var authenticator = Registry.Services.GetRequiredService<IAuthenticator>();
        var disposable    = new CompositeDisposable();
            
        var stateUpdates = authenticator
            .ObserveState()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler);
            
        stateUpdates.OfType<TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters>()
            .SelectMany(_ => authenticator.SetupParameters())
            .Accept()
            .DisposeWith(disposable);

        stateUpdates
            .Accept(state =>
            {
                switch (state)
                {
                    case TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters _:
                        GoToStartupPage();
                        break;
                
                    case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber _:
                    case TdApi.AuthorizationState.AuthorizationStateWaitCode _:
                    case TdApi.AuthorizationState.AuthorizationStateWaitPassword _:
                        GoToAuthenticationPage();
                        break;
            
                    case TdApi.AuthorizationState.AuthorizationStateReady _:
                        GoToWorkspacePage();
                        break;
                }
            })
            .DisposeWith(disposable);

        return disposable;
    }
    
    private void GoToStartupPage()
    {
        StartupModel      ??= new StartupModel();
        PageIndex           = (int) Page.Initial;
        WorkspaceModel      = null;
        AuthenticationModel = null;
    }

    private void GoToAuthenticationPage()
    {
        AuthenticationModel ??= new AuthenticationViewModel();
        PageIndex             = (int) Page.Authentication;
        StartupModel          = null;
        WorkspaceModel        = null;
    }

    private void GoToWorkspacePage()
    {
        WorkspaceModel    ??= new WorkspaceModel();
        PageIndex           = (int) Page.Workspace;
        StartupModel        = null;
        AuthenticationModel = null;
    }
}