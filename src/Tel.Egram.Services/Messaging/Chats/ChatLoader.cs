using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Chats;

public class ChatLoader(IAgent agent) : IChatLoader
{
    private const long PromoChatId = -1001316949630L;

    public IObservable<Chat> LoadChat(long chatId) => agent.Execute(new TdApi.GetChat { ChatId = chatId }).SelectSeq(chat =>
    {
        if (chat.Type is not TdApi.ChatType.ChatTypePrivate type) return Observable.Return(new Chat { ChatData = chat });
        
        return GetUser(type.UserId).Select(user => new Chat
        {
            ChatData = chat,
            User     = user
        });
    });

    public IObservable<Chat> LoadChats() => GetAllChats().SelectSeq(chat =>
    {
        if (chat.Type is not TdApi.ChatType.ChatTypePrivate type) return Observable.Return(new Chat { ChatData = chat });
        
        return GetUser(type.UserId).Select(user => new Chat
        {
            ChatData = chat,
            User     = user
        });

    });

    public IObservable<Chat> LoadChannels() => LoadChats().Where(chat =>
        chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup { IsChannel: true }
    );

    public IObservable<Chat> LoadDirects() => LoadChats().Where(chat => 
        chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate && chat.User is { Type: TdApi.UserType.UserTypeRegular }
    );

    public IObservable<Chat> LoadGroups() => LoadChats().Where(chat => 
        chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType
            ? !supergroupType.IsChannel
            : chat.ChatData.Type is TdApi.ChatType.ChatTypeBasicGroup
    );

    public IObservable<Chat> LoadBots() => LoadChats().Where(chat => 
        chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate && chat.User is { Type: TdApi.UserType.UserTypeBot }
    );

    public IObservable<Chat> LoadPromo() => agent.Execute(new TdApi.GetChat { ChatId = PromoChatId }).SelectSeq(chat =>
    {
        if (chat.Type is not TdApi.ChatType.ChatTypePrivate type) return Observable.Return(new Chat { ChatData = chat });
        
        return GetUser(type.UserId).Select(user => new Chat
        {
            ChatData = chat,
            User = user
        });
    });

    private IObservable<TdApi.User> GetUser(long id) => agent.Execute(new TdApi.GetUser
    {
        UserId = id
    });

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
            .SelectSeq(chatId => agent.Execute(new TdApi.GetChat { ChatId = chatId }));
    }
}