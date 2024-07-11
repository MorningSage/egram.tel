using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Application;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Utils.TdLib;
using Tel.Egran.ViewModels.Authentication;
using Tel.Egran.ViewModels.Popups;
using Tel.Egran.ViewModels.Workspace;

namespace Tel.Egran.ViewModels.Application;

public partial class MainWindowViewModel : AbstractViewModelBase
{
    private readonly IAuthenticator _authenticator;
    private readonly AuthenticationViewModel _authenticationViewModel;
    private readonly WorkspaceViewModel _workspaceViewModel;

    [ObservableProperty] private StartupModel? _startupModel;
    [ObservableProperty] private AuthenticationViewModel? _authenticationModel;
    [ObservableProperty] private WorkspaceViewModel? _workspaceModel;
    [ObservableProperty] private PopupViewModel? _popupModel;
    [ObservableProperty] private int _pageIndex;
    [ObservableProperty] private string _windowTitle = string.Empty;
    [ObservableProperty] private string _connectionState = "Initializing...";
    
    // ToDo: Eventually, would be nice to display popup view automatically if Length of popups > 0
    [ObservableProperty] private bool _popupVisible;

    public MainWindowViewModel(IPopupController popupController, IAgent agent, IAuthenticator authenticator, AuthenticationViewModel authenticationViewModel, WorkspaceViewModel workspaceViewModel)
    {
        _authenticator           = authenticator;
        _authenticationViewModel = authenticationViewModel;
        _workspaceViewModel      = workspaceViewModel;
        
        BindAuthentication();
        
        /* Subscribe to changes in the connection state to Telegram servers and store a readable string for the View */
        agent.Updates.OfType<TdApi.Update.UpdateConnectionState>().Subscribe(update => ConnectionState = update.State switch
        {
            TdApi.ConnectionState.ConnectionStateConnecting        => "Connecting...",
            TdApi.ConnectionState.ConnectionStateConnectingToProxy => "Connecting to proxy...",
            TdApi.ConnectionState.ConnectionStateReady             => "Ready.",
            TdApi.ConnectionState.ConnectionStateUpdating          => "Updating...",
            TdApi.ConnectionState.ConnectionStateWaitingForNetwork => "Waiting for network...",
            _                                                      => update.State.GetType().Name
        });
        
        /* Subscribe to the trigger for when the popup is changed */
        popupController.Trigger.Subscribe(context =>
        {
            PopupVisible = context is not null;
            PopupModel   = context is not null ? new PopupViewModel(context, popupController) : null;
        });
    }
    
    private void BindAuthentication()
    {
        var stateUpdates = _authenticator.ObserveState();

        stateUpdates.OfType<TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters>()
            .SelectMany(_ => _authenticator.SetupParameters())
            .Subscribe();

        stateUpdates.Subscribe(state =>
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
        });
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
        AuthenticationModel ??= _authenticationViewModel;
        PageIndex             = (int) Page.Authentication;
        StartupModel          = null;
        WorkspaceModel        = null;
    }
    
    private void GoToWorkspacePage()
    {
        WorkspaceModel    ??= _workspaceViewModel;
        PageIndex           = (int) Page.Workspace;
        StartupModel        = null;
        AuthenticationModel = null;
    }
}