using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Catalog.Entries;

public abstract partial class EntryViewModel : AbstractViewModelBase
{
    [ObservableProperty] private long _id;
    [ObservableProperty] private int _order;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private Avatar? _avatar;
    [ObservableProperty] private bool _hasUnread;
    [ObservableProperty] private string _unreadCount = string.Empty;
    
    protected EntryViewModel(IAvatarLoader avatarLoader)
    {
        Avatar = this switch
        {
            HomeEntryViewModel                          => avatarLoader.GetAvatar(AvatarKind.Home, AvatarSize.Small),
            ChatEntryViewModel chatEntryModel           => avatarLoader.GetAvatar(chatEntryModel.Chat.ChatData, AvatarSize.Small),
            AggregateEntryViewModel aggregateEntryModel => avatarLoader.GetAvatar(new TdApi.Chat { Id = aggregateEntryModel.Aggregate.Id }, AvatarSize.Small),
            _                                           => null
        };

        if (!(Avatar?.IsFallback ?? true)) return;
        
        var loadedAvatar = this switch
        {
            HomeEntryViewModel                          => avatarLoader.LoadAvatar(AvatarKind.Home, AvatarSize.Small),
            ChatEntryViewModel chatEntryModel           => avatarLoader.LoadAvatar(chatEntryModel.Chat.ChatData, AvatarSize.Small),
            AggregateEntryViewModel aggregateEntryModel => avatarLoader.LoadAvatar(new TdApi.Chat { Id = aggregateEntryModel.Aggregate.Id }, AvatarSize.Small),
            _                                           => Observable.Empty<Avatar>()
        };

        loadedAvatar.SafeSubscribe(a => Avatar = a);
    }
}