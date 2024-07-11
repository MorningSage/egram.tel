using System.ComponentModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Notifications;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.ViewModels.Messaging.Catalog;
using Tel.Egram.ViewModels.Messaging.Catalog.Entries;
using Tel.Egram.ViewModels.Messaging.Editor;
using Tel.Egram.ViewModels.Messaging.Explorer;
using Tel.Egram.ViewModels.Messaging.Explorer.Homepage;
using Tel.Egram.ViewModels.Messaging.Informer;
using Tel.Egram.ViewModels.Notifications;

namespace Tel.Egram.ViewModels.Messaging;

public partial class MessengerViewModel : AbstractViewModelBase
{
    private readonly IObservable<EntryViewModel?> _changedEvent;

    private readonly IChatLoader _chatLoader;
    private readonly IAvatarLoader _avatarLoader;
    private readonly IMessageSender _messageSender;
    private readonly IMessageLoader _messageLoader;
    private readonly INotificationSource _notificationSource;
    private readonly INotificationController _notificationController;
    private readonly IMessageModelFactory _messageModelFactory;

    [ObservableProperty] private CatalogViewModel _catalogViewModel;
    [ObservableProperty] private InformerViewModel _informerViewModel = InformerViewModel.Hidden();
    [ObservableProperty] private ExplorerViewModel _explorerViewModel = ExplorerViewModel.Hidden();
    [ObservableProperty] private HomepageViewModel _homepageViewModel = HomepageViewModel.Hidden();
    [ObservableProperty] private EditorViewModel _editorModel         = EditorViewModel.Hidden();

    public MessengerViewModel(Section section, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader, IMessageSender messageSender, IMessageLoader messageLoader, INotificationSource notificationSource, INotificationController notificationController, IMessageModelFactory messageModelFactory)
    {
        _chatLoader             = chatLoader;
        _avatarLoader           = avatarLoader;
        _messageSender          = messageSender;
        _messageLoader          = messageLoader;
        _notificationSource     = notificationSource;
        _notificationController = notificationController;
        _messageModelFactory    = messageModelFactory;

        /* Allows easy access to the handling of Entry Selection changes */
        _changedEvent = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h)
            .Select(pattern => pattern.EventArgs)
            .Where(args => args.PropertyName is nameof(CatalogViewModel.SelectedEntry))
            .Select(_ => CatalogViewModel.SelectedEntry);
        
        CatalogViewModel  = new CatalogViewModel(section, chatLoader, chatUpdater, avatarLoader);

        BindInformer();
        BindExplorer();
        BindHome();
        BindEditor();
        
        BindNotifications();
    }

    private void BindInformer() => _changedEvent.Subscribe(entry => InformerViewModel = entry switch
    {
        ChatEntryViewModel chatEntryModel           => new InformerViewModel(chatEntryModel.Chat, _avatarLoader),
        AggregateEntryViewModel aggregateEntryModel => new InformerViewModel(aggregateEntryModel.Aggregate),
        HomeEntryViewModel                          => InformerViewModel.Hidden(),
        _                                           => InformerViewModel
    });

    private void BindExplorer() => _changedEvent.Subscribe(entry => ExplorerViewModel = entry switch
    {
        ChatEntryViewModel chatEntryModel => new ExplorerViewModel(chatEntryModel.Chat, _chatLoader, _messageLoader, _messageModelFactory),
        AggregateEntryViewModel           => ExplorerViewModel,
        HomeEntryViewModel                => ExplorerViewModel.Hidden(),
        _                                 => ExplorerViewModel
    });

    private void BindHome() => _changedEvent.Subscribe(entry => HomepageViewModel = entry switch
    {
        HomeEntryViewModel => new HomepageViewModel(),
        _                  => HomepageViewModel.Hidden()
    });

    private void BindEditor() => _changedEvent.Subscribe(entry => EditorModel = entry switch
    {
        ChatEntryViewModel chatEntryModel => new EditorViewModel(chatEntryModel.Chat, _messageSender),
        _                                 => EditorViewModel.Hidden()
    });

    private void BindNotifications() => _notificationSource.MessagesNotifications().Merge(_notificationSource.ChatNotifications()).Buffer(TimeSpan.FromSeconds(2)).SafeSubscribe(notifications =>
    {
        switch (notifications.Count)
        {
            case 0:
                break;
                
            case 1:
                _notificationController.Show(NotificationViewModel.FromNotification(notifications[0]));
                break;
                
            default:
                _notificationController.Show(NotificationViewModel.FromNotificationList(notifications));
                break;
        }
    });
}