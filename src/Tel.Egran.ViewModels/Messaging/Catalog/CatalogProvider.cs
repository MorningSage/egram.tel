using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egran.ViewModels.Messaging.Catalog;

public static class CatalogProvider
{
    private static readonly IChatLoader ChatLoader = Registry.Services.GetRequiredService<IChatLoader>();
    private static readonly IChatUpdater ChatUpdater = Registry.Services.GetRequiredService<IChatUpdater>();
    
    private static readonly Dictionary<long, EntryViewModel> EntryStore = [];
    private static readonly SourceCache<EntryViewModel, long> Chats = new(m => m.Id);
    
    public static IDisposable BindChats(this CatalogViewModel viewModel, Section section)
    {
        var entries = viewModel.Entries;
        var filter  = viewModel.FilterController;
        var sorting = viewModel.SortingController;

        var disposable = new CompositeDisposable();

        viewModel.LoadHome(section).DisposeWith(disposable);
            
        Chats.Connect()
            .Filter(filter)
            .Sort(sorting)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(entries)
            .Accept()
            .DisposeWith(disposable);
            
        BindOrderUpdates().DisposeWith(disposable);
        BindEntryUpdates().DisposeWith(disposable);
        LoadChats().DisposeWith(disposable);

        return disposable;
    }
    
    private static IDisposable LoadHome(this CatalogViewModel viewModel, Section section)
    {
        if (section == Section.Home)
        {
            viewModel.Entries.Add(HomeEntryViewModel.Instance);
            viewModel.SelectedEntry = HomeEntryViewModel.Instance;
            
            Chats.AddOrUpdate(HomeEntryViewModel.Instance);
        }
        
        return Disposable.Empty;
    }

    /// <summary>
    /// Load chats into observable cache
    /// </summary>
    private static IDisposable LoadChats()
    {
        return ChatLoader.LoadChats()
            .Select(GetChatEntryModel)
            .Aggregate(new List<EntryViewModel>(), (list, model) =>
            {
                model.Order = list.Count;
                list.Add(model);
                return list;
            })
            .Accept(entries =>
            {
                entries.Insert(0, HomeEntryViewModel.Instance);
                    
                Chats.EditDiff(entries, (m1, m2) => m1.Id == m2.Id);
                Chats.Refresh();
            });
    }

    /// <summary>
    /// Subscribe to updates that involve order change
    /// </summary>
    private static IDisposable BindOrderUpdates()
    {
        return ChatUpdater.GetOrderUpdates()
            .Buffer(TimeSpan.FromSeconds(2))
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(changes => changes.Count > 0)
            .SelectMany(_ => ChatLoader.LoadChats()
                .Select(GetChatEntryModel)
                .ToList())
            .Do(entries =>
            {
                for (var i = 0; i < entries.Count; i++) entries[i].Order = i;
            })
            .Accept(entries =>
            {
                entries.Insert(0, HomeEntryViewModel.Instance);
                Chats.EditDiff(entries, (m1, m2) => m1.Id == m2.Id);
                Chats.Refresh();
            });
    }

    /// <summary>
    /// Subscribe to updates for individual entries
    /// </summary>
    private static IDisposable BindEntryUpdates()
    {
        return ChatUpdater.GetChatUpdates()
            .Buffer(TimeSpan.FromSeconds(1))
            .SelectMany(chats => chats)
            .Select(chat => new { Chat = chat, Entry = GetChatEntryModel(chat) })
            .Accept(item => { UpdateChatEntryModel((ChatEntryViewModel)item.Entry, item.Chat); });
    }

    private static EntryViewModel GetChatEntryModel(Chat chat)
    {
        var chatData = chat.ChatData;
            
        if (!EntryStore.TryGetValue(chatData.Id, out var entry))
        {
            entry = new ChatEntryViewModel { Chat = chat };
            
            UpdateChatEntryModel((ChatEntryViewModel) entry, chat);
                
            EntryStore.Add(chatData.Id, entry);
        }

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