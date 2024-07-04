using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Chats;

public class FeedLoader : IFeedLoader
{
    private readonly IAgent _agent;

    public FeedLoader(IAgent agent)
    {
        _agent = agent;
    }
        
    public IObservable<Aggregate> LoadAggregate()
    {
        return GetAllChats(new List<TdApi.Chat>())
            .Where(chat =>
            {
                var type = chat.Type as TdApi.ChatType.ChatTypeSupergroup;
                return !(type is null) && type.IsChannel;
            })
            .Select(chat => new Chat
            {
                ChatData = chat
            })
            .CollectToList()
            .Select(list => new Aggregate(list));
    }

    public IObservable<Chat> LoadChat(long chatId)
    {
        return _agent.Execute(new TdApi.GetChat
            {
                ChatId = chatId
            })
            .Select(chat => new Chat
            {
                ChatData = chat
            });
    }

    private IObservable<TdApi.Chat> GetAllChats(
        List<TdApi.Chat> chats,
        long offsetOrder = long.MaxValue,
        long offsetChatId = 0)
    {
        int limit = 100;
            
        return GetChats(limit);
    }

    private IObservable<TdApi.Chat> GetChats(int limit)
    {
        return _agent.Execute(new TdApi.GetChats
            {
                Limit = limit
            })
            .SelectMany(result => result.ChatIds)
            .SelectMany(chatId => _agent.Execute(new TdApi.GetChat
            {
                ChatId = chatId
            }));
    }
}