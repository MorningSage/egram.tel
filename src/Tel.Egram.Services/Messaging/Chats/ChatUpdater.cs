using System.Reactive;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Chats;

public class ChatUpdater(IAgent agent) : IChatUpdater
{
    public IObservable<Unit> GetOrderUpdates()
    {
        var newUpdates     = agent.Updates.OfType<TdApi.Update.UpdateNewChat>().Select(_ => Unit.Default);
        var orderUpdates   = agent.Updates.OfType<TdApi.Update.UpdateChatPosition>().Select(_ => Unit.Default);
        var messageUpdates = agent.Updates.OfType<TdApi.Update.UpdateChatLastMessage>().Select(_ => Unit.Default);
        var draftUpdates   = agent.Updates.OfType<TdApi.Update.UpdateChatDraftMessage>().Select(_ => Unit.Default);

        return newUpdates
            .Merge(orderUpdates)
            .Merge(messageUpdates)
            .Merge(draftUpdates);
    }

    public IObservable<Chat> GetChatUpdates()
    {
        var titleUpdates   = agent.Updates.OfType<TdApi.Update.UpdateChatTitle>().SelectMany(u => GetChat(u.ChatId));
        var photoUpdates   = agent.Updates.OfType<TdApi.Update.UpdateChatPhoto>().SelectMany(u => GetChat(u.ChatId));
        var inboxUpdates   = agent.Updates.OfType<TdApi.Update.UpdateChatReadInbox>().SelectMany(u => GetChat(u.ChatId));
        var messageUpdates = agent.Updates.OfType<TdApi.Update.UpdateChatLastMessage>().SelectMany(u => GetChat(u.ChatId));
                
        return titleUpdates
            .Merge(photoUpdates)
            .Merge(inboxUpdates)
            .Merge(messageUpdates);
    }

    private IObservable<Chat> GetChat(long chatId) => agent.Execute(new TdApi.GetChat { ChatId = chatId }).SelectMany(chat =>
    {
        if (chat.Type is not TdApi.ChatType.ChatTypePrivate type) return Observable.Return(new Chat { User = null, ChatData = chat });
        
        return GetUser(type.UserId).Select(user => new Chat
        {
            ChatData = chat,
            User     = user
        });
    });

    private IObservable<TdApi.User> GetUser(long id) => agent.Execute(new TdApi.GetUser { UserId = id });
}