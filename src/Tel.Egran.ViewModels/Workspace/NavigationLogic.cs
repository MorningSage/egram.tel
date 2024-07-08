using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Workspace;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging;
using Tel.Egran.ViewModels.Settings;
using Tel.Egran.ViewModels.Workspace.Navigation;

namespace Tel.Egran.ViewModels.Workspace;

public static class NavigationLogic
{
    public static IDisposable BindNavigation(this WorkspaceModel model)
    {
        model.NavigationModel = new NavigationModel();
            
        return model.NavigationModel.WhenAnyValue(m => m.SelectedTabIndex)
            .Select(index => (ContentKind)index)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(kind =>
            {
                switch (kind)
                {
                    case ContentKind.Settings:
                        InitSettings(model);
                        break;
                        
                    default:
                        InitMessenger(model, kind);
                        break;
                }
            });
    }

    private static void InitMessenger(WorkspaceModel model, ContentKind kind)
    {
        var section = (Section) kind;
            
        model.SettingsModel = null;
        model.MessengerModel = new MessengerViewModel(section);
    }

    private static void InitSettings(WorkspaceModel model)
    {
        model.ContentIndex = 1;

        model.MessengerModel = null;
        model.SettingsModel = new SettingsModel();
    }
}