using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Chats;

public class FeedLoader(IAgent agent) : IFeedLoader
{
    public IObservable<Aggregate> LoadAggregate() => GetAllChats()
        .Where(chat => chat is { Type: TdApi.ChatType.ChatTypeSupergroup { IsChannel: true }})
        .Select(chat => new Chat { ChatData = chat })
        .CollectToList()
        .Select(list => new Aggregate(list));

    public IObservable<Chat> LoadChat(long chatId) => agent
        .Execute(new TdApi.GetChat { ChatId = chatId })
        .Select(chat => new Chat { ChatData = chat });

    private IObservable<TdApi.Chat> GetAllChats()
    {
        const int limit = 100;
        return GetChats(limit);
    }
    
    private IObservable<TdApi.Chat> GetChats(int limit)
    {
        // ToDo: This is not the way we should be getting chats.  Update to LoadChats
        return agent.Execute(new TdApi.GetChats { Limit = limit })
            .SelectMany(result => result.ChatIds)
            .SelectMany(chatId => agent.Execute(new TdApi.GetChat { ChatId = chatId }));
    }
}