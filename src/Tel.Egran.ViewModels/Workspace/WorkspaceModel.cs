using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Notifications;
using Tel.Egran.ViewModels.Messaging;
using Tel.Egran.ViewModels.Settings;
using Tel.Egran.ViewModels.Workspace.Navigation;

namespace Tel.Egran.ViewModels.Workspace;

[AddINotifyPropertyChangedInterface]
public class WorkspaceModel : IActivatableViewModel
{
    public NavigationModel NavigationModel { get; set; }
        
    public MessengerViewModel? MessengerModel { get; set; }
        
    public SettingsModel? SettingsModel { get; set; }
        
    public int ContentIndex { get; set; }

    public WorkspaceModel(IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IUserLoader userLoader, IMessageModelFactory messageModelFactory)
    {
        this.WhenActivated(disposables =>
        {
            this.BindNavigation(chatLoader, chatUpdater, avatarLoader, messageSender, messageLoader, notificationSource, notificationController, userLoader, messageModelFactory).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}