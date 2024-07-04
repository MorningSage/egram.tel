using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Chats;

public class ChatLoader : IChatLoader
{
    private readonly IAgent _agent;
    private readonly long _promoChatId;

    public ChatLoader(IAgent agent)
    {
        _agent = agent;
        _promoChatId = -1001316949630L;
    }

    public IObservable<Chat> LoadChat(long chatId)
    {
        return _agent.Execute(new TdApi.GetChat
            {
                ChatId = chatId
            })
            .SelectSeq(chat =>
            {
                if (chat.Type is TdApi.ChatType.ChatTypePrivate type)
                {
                    return GetUser(type.UserId)
                        .Select(user => new Chat
                        {
                            ChatData = chat,
                            User = user
                        });
                }

                return Observable.Return(new Chat
                {
                    ChatData = chat
                });
            });
    }

    public IObservable<Chat> LoadChats()
    {
        return GetAllChats(new List<TdApi.Chat>())
            .SelectSeq(chat =>
            {
                if (chat.Type is TdApi.ChatType.ChatTypePrivate type)
                {
                    return GetUser(type.UserId)
                        .Select(user => new Chat
                        {
                            ChatData = chat,
                            User = user
                        });
                }
                    
                return Observable.Return(new Chat
                {
                    ChatData = chat
                });
            });
    }

    public IObservable<Chat> LoadChannels()
    {
        return LoadChats().Where(chat =>
        {
            if (chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
            {
                return supergroupType.IsChannel;
            }
            return false;
        });
    }

    public IObservable<Chat> LoadDirects()
    {
        return LoadChats().Where(chat =>
        {
            if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
            {
                return chat.User != null &&
                       chat.User.Type is TdApi.UserType.UserTypeRegular;
            }
            return false;
        });
    }

    public IObservable<Chat> LoadGroups()
    {
        return LoadChats().Where(chat =>
        {
            if (chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
            {
                return !supergroupType.IsChannel;
            }

            return chat.ChatData.Type is TdApi.ChatType.ChatTypeBasicGroup;
        });
    }

    public IObservable<Chat> LoadBots()
    {
        return LoadChats().Where(chat =>
        {
            if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
            {
                return chat.User != null &&
                       chat.User.Type is TdApi.UserType.UserTypeBot;
            }
            return false;
        });
    }

    public IObservable<Chat> LoadPromo()
    {
        return _agent.Execute(new TdApi.GetChat
            {
                ChatId = _promoChatId
            })
            .SelectSeq(chat =>
            {
                if (chat.Type is TdApi.ChatType.ChatTypePrivate type)
                {
                    return GetUser(type.UserId)
                        .Select(user => new Chat
                        {
                            ChatData = chat,
                            User = user
                        });
                }
                        
                return Observable.Return(new Chat
                {
                    ChatData = chat
                });
            });
    }

    private IObservable<TdApi.User> GetUser(long id)
    {
        return _agent.Execute(new TdApi.GetUser
        {
            UserId = id
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
        // ToDo: This is not the way we should be getting chats.  Update to LoadChats
        return _agent.Execute(new TdApi.GetChats
            {
                Limit = limit
            })
            .SelectMany(result => result.ChatIds)
            .SelectSeq(chatId => _agent.Execute(new TdApi.GetChat
            {
                ChatId = chatId
            }));
    }
}