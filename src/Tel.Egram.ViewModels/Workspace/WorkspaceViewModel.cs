using System.ComponentModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Workspace;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Notifications;
using Tel.Egram.ViewModels.Messaging;
using Tel.Egram.ViewModels.Settings;
using Tel.Egram.ViewModels.Workspace.Navigation;

namespace Tel.Egram.ViewModels.Workspace;

public partial class WorkspaceViewModel : AbstractViewModelBase
{
    [ObservableProperty] private NavigationModel _navigationModel;
    [ObservableProperty] private MessengerViewModel? _messengerModel;
    [ObservableProperty] private SettingsViewModel? _settingsModel;
    [ObservableProperty] private int _contentIndex;

    public WorkspaceViewModel(IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IUserLoader userLoader, IMessageModelFactory messageModelFactory)
    {
        _navigationModel = new NavigationModel(avatarLoader, userLoader);

        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => _navigationModel.PropertyChanged += h, h => _navigationModel.PropertyChanged -= h)
            .Where(pattern => pattern.EventArgs.PropertyName is nameof(NavigationModel.SelectedTabIndex))
            .Select(_ => (ContentKind)_navigationModel.SelectedTabIndex)
            .Subscribe(kind =>
            {
                switch (kind)
                {
                    case ContentKind.Settings:
                        ContentIndex   = 1;
                        MessengerModel = null;
                        SettingsModel  = new SettingsViewModel();
                        break;
                    default:
                        var section = (Section) kind;
                        SettingsModel = null;
                        MessengerModel = new MessengerViewModel(section, chatLoader, chatUpdater, avatarLoader, messageSender, messageLoader, notificationSource, notificationController, messageModelFactory);
                        break;
                }
            });
    }
}