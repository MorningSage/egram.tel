using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egran.ViewModels.Messaging.Catalog;

public static class CatalogFilter
{
    public static IDisposable BindCatalogFilter(this CatalogViewModel viewModel, Section section)
    {
        return viewModel.WhenAnyValue(m => m.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Accept(text =>
            {
                var sorting = GetSorting(e => e.Order);
                var filter  = GetFilter(section);
                    
                if (!string.IsNullOrWhiteSpace(text))
                {
                    sorting = GetSorting(e => e.Title);
                    filter = entry => entry.Title.Contains(text) && GetFilter(section)(entry);
                }
                    
                viewModel.SortingController.OnNext(sorting);
                viewModel.FilterController.OnNext(filter);
            });
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

    private static IComparer<EntryViewModel> GetSorting(Func<EntryViewModel, IComparable> f)
    {
        return SortExpressionComparer<EntryViewModel>.Ascending(f);
    }
        
    private static bool All(EntryViewModel viewModel)
    {
        return true;
    }
        
    private static bool BotFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
            
        if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
        {
            return chat.User is { Type: TdApi.UserType.UserTypeBot };
        }

        return false;
    }

    private static bool DirectFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
            
        if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
        {
            return chat.User is { Type: TdApi.UserType.UserTypeRegular };
        }
        
        return false;
    }

    private static bool GroupFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
        
        if (chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
        {
            return !supergroupType.IsChannel;
        }

        return chat.ChatData.Type is TdApi.ChatType.ChatTypeBasicGroup;
    }

    private static bool ChannelFilter(EntryViewModel viewModel)
    {
        if (viewModel is not ChatEntryViewModel chatEntryModel) return false;
        
        var chat = chatEntryModel.Chat;
        
        if (chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
        {
            return supergroupType.IsChannel;
        }
        
        return false;
    }
}