using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egran.ViewModels.Messaging.Catalog;

public partial class CatalogViewModel : AbstractViewModelBase
{
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private EntryViewModel? _selectedEntry;
    [ObservableProperty] private ObservableCollectionExtended<EntryViewModel> _entries = [];
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private Subject<IComparer<EntryViewModel>> _sortingController = new();
    [ObservableProperty] private Subject<Func<EntryViewModel, bool>> _filterController = new();

    public CatalogViewModel(Section section, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader)
    {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h)
            .Where(pattern => pattern.EventArgs.PropertyName is nameof(SearchText))
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Subscribe(_ =>
            {
                var sorting = GetSorting(e => e.Order);
                var filter  = GetFilter(section);
                    
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    sorting = GetSorting(e => e.Title);
                    filter = entry => entry.Title.Contains(SearchText) && GetFilter(section)(entry);
                }
                    
                SortingController.OnNext(sorting);
                FilterController.OnNext(filter);
            });
        
        
        this.BindChats(section, chatLoader, chatUpdater, avatarLoader);
    }

    private static IComparer<EntryViewModel> GetSorting(Func<EntryViewModel, IComparable> f)
    {
        return SortExpressionComparer<EntryViewModel>.Ascending(f);
    }
    
    private static Func<EntryViewModel, bool> GetFilter(Section section) => section switch
    {
        Section.Bots     => BotFilter,
        Section.Channels => ChannelFilter,
        Section.Groups   => GroupFilter,
        Section.Directs  => DirectFilter,
        Section.Home     => All,
        _                => All
    };
    
    private static bool All(EntryViewModel viewModel) => true;
        
    private static bool BotFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
            
        if (chat.ChatData?.Type is TdApi.ChatType.ChatTypePrivate)
        {
            return chat.User is { Type: TdApi.UserType.UserTypeBot };
        }

        return false;
    }

    private static bool DirectFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
            
        if (chat.ChatData?.Type is TdApi.ChatType.ChatTypePrivate)
        {
            return chat.User is { Type: TdApi.UserType.UserTypeRegular };
        }
        
        return false;
    }

    private static bool GroupFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
        
        if (chat.ChatData?.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
        {
            return !supergroupType.IsChannel;
        }

        return chat.ChatData?.Type is TdApi.ChatType.ChatTypeBasicGroup;
    }

    private static bool ChannelFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
        
        if (chat.ChatData?.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
        {
            return supergroupType.IsChannel;
        }
        
        return false;
    }
}