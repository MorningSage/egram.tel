using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Workspace;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Notifications;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging;
using Tel.Egran.ViewModels.Settings;
using Tel.Egran.ViewModels.Workspace.Navigation;

namespace Tel.Egran.ViewModels.Workspace;

public static class NavigationLogic
{
    public static IDisposable BindNavigation(this WorkspaceModel model, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IUserLoader userLoader, IMessageModelFactory messageModelFactory)
    {
        model.NavigationModel = new NavigationModel(avatarLoader, userLoader);
            
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
                        InitMessenger(model, kind, chatLoader, chatUpdater, avatarLoader, messageSender, messageLoader, notificationSource, notificationController, messageModelFactory);
                        break;
                }
            });
    }

    private static void InitMessenger(WorkspaceModel model, ContentKind kind, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IMessageModelFactory messageModelFactory)
    {
        var section = (Section) kind;
            
        model.SettingsModel = null;
        model.MessengerModel = new MessengerViewModel(section, chatLoader, chatUpdater, avatarLoader, messageSender, messageLoader, notificationSource, notificationController, messageModelFactory);
    }

    private static void InitSettings(WorkspaceModel model)
    {
        model.ContentIndex = 1;

        model.MessengerModel = null;
        model.SettingsModel = new SettingsModel();
    }
}