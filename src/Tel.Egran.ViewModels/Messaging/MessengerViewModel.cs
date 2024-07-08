using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Notifications;
using Tel.Egran.ViewModels.Messaging.Catalog;
using Tel.Egran.ViewModels.Messaging.Editor;
using Tel.Egran.ViewModels.Messaging.Explorer;
using Tel.Egran.ViewModels.Messaging.Explorer.Homepage;
using Tel.Egran.ViewModels.Messaging.Informer;

namespace Tel.Egran.ViewModels.Messaging;

[AddINotifyPropertyChangedInterface]
public class MessengerViewModel : IActivatableViewModel
{   
    public CatalogViewModel CatalogViewModel { get; set; }
        
    public InformerViewModel InformerViewModel { get; set; }
        
    public ExplorerViewModel ExplorerViewModel { get; set; }
        
    public HomepageViewModel HomepageViewModel { get; set; }
        
    public EditorViewModel EditorModel { get; set; }

    public MessengerViewModel(Section section, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IMessageModelFactory messageModelFactory)
    {
        this.WhenActivated(disposables =>
        {
            this.BindCatalog(section, chatLoader, chatUpdater, avatarLoader).DisposeWith(disposables);
            this.BindInformer(avatarLoader).DisposeWith(disposables);
            this.BindExplorer(chatLoader, messageLoader, messageModelFactory).DisposeWith(disposables);
            this.BindHome().DisposeWith(disposables);
            this.BindEditor(messageSender).DisposeWith(disposables);
            this.BindNotifications(notificationSource, notificationController).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}