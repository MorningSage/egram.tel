using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using TdLib;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public static class AvatarLoadingLogic
{
    private static readonly IAvatarLoader AvatarLoader = Registry.Services.GetRequiredService<IAvatarLoader>();

    public static IDisposable BindAvatarLoading(this EntryViewModel viewModel)
    {
        if (viewModel.Avatar != null) return Disposable.Empty;
        
        viewModel.Avatar = GetAvatar(AvatarLoader, viewModel);

        return viewModel.Avatar?.IsFallback is null or true
            ? LoadAvatar(AvatarLoader, viewModel).Accept(avatar => { viewModel.Avatar = avatar; })
            : Disposable.Empty;
    }

    private static Avatar? GetAvatar(IAvatarLoader avatarLoader, EntryViewModel entryView)
    {
        return entryView switch
        {
            HomeEntryViewModel                          => avatarLoader.GetAvatar(AvatarKind.Home, AvatarSize.Small),
            ChatEntryViewModel chatEntryModel           => avatarLoader.GetAvatar(chatEntryModel.Chat.ChatData, AvatarSize.Small),
            AggregateEntryViewModel aggregateEntryModel => avatarLoader.GetAvatar(new TdApi.Chat { Id = aggregateEntryModel.Aggregate.Id }, AvatarSize.Small),
            _                                       => null
        };
    }

    private static IObservable<Avatar> LoadAvatar(IAvatarLoader avatarLoader, EntryViewModel entryView) => entryView switch
    {
        HomeEntryViewModel                          => avatarLoader.LoadAvatar(AvatarKind.Home, AvatarSize.Small),
        ChatEntryViewModel chatEntryModel           => avatarLoader.LoadAvatar(chatEntryModel.Chat.ChatData, AvatarSize.Small),
        AggregateEntryViewModel aggregateEntryModel => avatarLoader.LoadAvatar(new TdApi.Chat { Id = aggregateEntryModel.Aggregate.Id }, AvatarSize.Small),
        _                                       => Observable.Empty<Avatar>()
    };
}