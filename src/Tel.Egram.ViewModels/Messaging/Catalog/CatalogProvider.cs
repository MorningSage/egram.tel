using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egram.ViewModels.Messaging.Catalog;

public static class CatalogProvider
{
    private static readonly Dictionary<long, EntryViewModel> EntryStore = [];
    private static readonly SourceCache<EntryViewModel, long> Chats = new(m => m.Id);
    
    public static IDisposable BindChats(this CatalogViewModel viewModel, Section section, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader)
    {
        var entries = viewModel.Entries;
        var filter  = viewModel.FilterController;
        var sorting = viewModel.SortingController;

        var disposable = new CompositeDisposable();

        viewModel.LoadHome(section, avatarLoader);
            
        Chats.Connect()
            .Filter(filter)
            .Sort(sorting)
            .Bind(entries)
            .Subscribe();
            
        BindOrderUpdates(chatLoader, chatUpdater, avatarLoader);
        BindEntryUpdates(chatUpdater, avatarLoader);
        LoadChats(chatLoader, avatarLoader);

        return disposable;
    }
    
    private static void LoadHome(this CatalogViewModel viewModel, Section section, IAvatarLoader avatarLoader)
    {
        if (section != Section.Home) return;
        
        viewModel.Entries.Add(HomeEntryViewModel.GetInstance(avatarLoader));
        viewModel.SelectedEntry = HomeEntryViewModel.GetInstance(avatarLoader);
            
        Chats.AddOrUpdate(HomeEntryViewModel.GetInstance(avatarLoader));
    }

    /// <summary>
    /// Load chats into observable cache
    /// </summary>
    private static void LoadChats(IChatLoader chatLoader, IAvatarLoader avatarLoader)
    {
        chatLoader.LoadChats()
            .Select(chat => GetChatEntryModel(chat, avatarLoader))
            .Aggregate(new List<EntryViewModel>(), (list, model) =>
            {
                model.Order = list.Count;
                list.Add(model);
                return list;
            })
            .SafeSubscribe(entries =>
            {
                entries.Insert(0, HomeEntryViewModel.GetInstance(avatarLoader));
                    
                Chats.EditDiff(entries, (m1, m2) => m1.Id == m2.Id);
                Chats.Refresh();
            });
    }

    /// <summary>
    /// Subscribe to updates that involve order change
    /// </summary>
    private static void BindOrderUpdates(IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader)
    {
        chatUpdater.GetOrderUpdates()
            .Buffer(TimeSpan.FromSeconds(2))
            .Where(changes => changes.Count > 0)
            .SelectMany(_ => chatLoader.LoadChats()
                .Select(chat => GetChatEntryModel(chat, avatarLoader))
                .ToList())
            .Do(entries =>
            {
                for (var i = 0; i < entries.Count; i++) entries[i].Order = i;
            })
            .SafeSubscribe(entries =>
            {
                entries.Insert(0, HomeEntryViewModel.GetInstance(avatarLoader));
                Chats.EditDiff(entries, (m1, m2) => m1.Id == m2.Id);
                Chats.Refresh();
            });
    }

    /// <summary>
    /// Subscribe to updates for individual entries
    /// </summary>
    private static void BindEntryUpdates(IChatUpdater chatUpdater, IAvatarLoader avatarLoader)
    {
        chatUpdater.GetChatUpdates()
            .Buffer(TimeSpan.FromSeconds(1))
            .SelectMany(chats => chats)
            .Select(chat => new { Chat = chat, Entry = GetChatEntryModel(chat, avatarLoader) })
            .SafeSubscribe(item => UpdateChatEntryModel((ChatEntryViewModel)item.Entry, item.Chat));
    }

    private static EntryViewModel GetChatEntryModel(Chat chat, IAvatarLoader avatarLoader)
    {
        var chatData = chat.ChatData;

        if (EntryStore.TryGetValue(chatData.Id, out var entry)) return entry;
        
        entry = new ChatEntryViewModel(avatarLoader) { Chat = chat };
            
        UpdateChatEntryModel((ChatEntryViewModel) entry, chat);
                
        EntryStore.Add(chatData.Id, entry);

        return entry;
    }

    private static void UpdateChatEntryModel(ChatEntryViewModel entryView, Chat chat)
    {
        var chatData = chat.ChatData;

        entryView.Chat        = chat;
        entryView.Id          = chatData.Id;
        entryView.Title       = chatData.Title;
        entryView.HasUnread   = chatData.UnreadCount > 0;
        entryView.UnreadCount = chatData.UnreadCount.ToString();
    }
}