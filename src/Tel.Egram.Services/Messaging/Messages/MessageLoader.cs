using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Messages;

public class MessageLoader(IAgent agent) : IMessageLoader
{
    private const int Limit = 10;

    public IObservable<Message> LoadMessage(long chatId, long messageId)
    {
        var scope = new MessageLoaderScope(agent);
            
        return scope.GetMessage(chatId, messageId).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    public IObservable<Message> LoadMessages(Aggregate feed, AggregateLoadingState state)
    {
        var scope = new MessageLoaderScope(agent);
            
        return LoadAggregateMessages(feed, state).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    public IObservable<Message> LoadInitMessages(Chat chat, long fromMessageId, int limit)
    {
        if (chat.ChatData is not { } chatData) return Observable.Empty<Message>();
        
        var scope = new MessageLoaderScope(agent);
            
        return GetMessages(chatData, fromMessageId, limit, -(limit - 1) / 2).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    public IObservable<Message> LoadPrevMessages(Chat chat, long fromMessageId, int limit)
    {
        if (chat.ChatData is not { } chatData) return Observable.Empty<Message>();
        
        var scope = new MessageLoaderScope(agent);
            
        return GetMessages(chatData, fromMessageId, limit, 0).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    public IObservable<Message> LoadNextMessages(Chat chat, long fromMessageId, int limit)
    {
        if (chat.ChatData is not { } chatData) return Observable.Empty<Message>();
        
        var scope = new MessageLoaderScope(agent);
            
        return GetMessages(chatData, fromMessageId, limit, -(limit - 1)).Where(m => m.Id != fromMessageId).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    public IObservable<Message> LoadPinnedMessage(Chat chat)
    {
        if (chat.ChatData is not { } chatData) return Observable.Empty<Message>();
        
        var scope = new MessageLoaderScope(agent);
            
        return GetPinnedMessage(chatData).SelectSeq(m => MapToMessage(scope, m)).Finally(() =>
        {
            scope.Dispose();
        });
    }

    private IObservable<Message> MapToMessage(MessageLoaderScope scope, TdApi.Message msg, bool fetchReply = true)
    {
        return Observable.Return(new Message { MessageData = msg })
            .SelectSeq(message => scope.GetChat(msg.ChatId).Select(chat =>
            {
                // get chat data
                message.ChatData = chat;
                return message;
            }))
            .SelectSeq(message =>
            {
                // get sender data
                return message.MessageData.SenderId switch
                {
                    TdApi.MessageSender.MessageSenderUser senderUser when senderUser.UserId != 0L => scope.GetUser(senderUser.UserId).Select(user =>
                    {
                        message.UserData = user;
                        return message;
                    }),
                    TdApi.MessageSender.MessageSenderChat senderChat when senderChat.ChatId != 0L => scope.GetChat(senderChat.ChatId).Select(chat =>
                    {
                        // ToDo: "Senders" can be users or chats.  This needs to be updated to ensure everything still works
                        message.ChatData = chat;
                        return message;
                    }),
                    _ => Observable.Return(message)
                };
            })
            .SelectSeq(message =>
            {
                // get reply data
                return message.MessageData.ReplyTo switch
                {
                    TdApi.MessageReplyTo.MessageReplyToMessage messageReplyToMessage when fetchReply => scope.GetMessage(messageReplyToMessage.ChatId, messageReplyToMessage.MessageId)
                        .SelectSeq(m => MapToMessage(scope, m, false))
                        .Select(reply =>
                        {
                            message.ReplyMessage = reply;
                            return message;
                        }),
                    TdApi.MessageReplyTo.MessageReplyToStory messageReplyToStory when fetchReply =>
                        // ToDo: This may fail - are stories considered Messages?  Presumably not.
                        scope.GetMessage(messageReplyToStory.StorySenderChatId, messageReplyToStory.StoryId)
                            .SelectSeq(m => MapToMessage(scope, m, false))
                            .Select(reply =>
                            {
                                message.ReplyMessage = reply;
                                return message;
                            }),
                    _ => Observable.Return(message)
                };
            });
    }

    private IObservable<TdApi.Message> LoadAggregateMessages(Aggregate aggregate, AggregateLoadingState state)
    {
        var actualLimit = Limit;
            
        var list = aggregate.Chats.Select(f =>
        {
            if (f.ChatData is not { } chatData) return Observable.Empty<TdApi.Message>();
            
            var stackedCount = state.CountStackedMessages(chatData.Id);
                
            return Enumerable.Range(0, stackedCount)
                .Select(_ => state.PopMessageFromStack(chatData.Id)) // get stacked messages for this chat
                .ToObservable()
                .Concat(stackedCount < Limit
                    ? LoadChannelMessages(f, new ChatLoadingState // load messages from the server
                    {
                        LastMessageId = state.GetLastMessageId(chatData.Id)
                    }, Limit, 0)
                    : Observable.Empty<TdApi.Message>())
                .CollectToList()
                .Do(l =>
                {
                    // api has no guarantees about actual number of messages returned
                    // actual limit would be equal to min number of messages for all feeds
                    // unless it is zero
                    if (l.Count > 0 && l.Count < actualLimit)
                    {
                        actualLimit = l.Count;
                    }
                })
                .SelectMany(messages => messages);
        });
            
        return list.Merge().CollectToList().SelectMany(l =>
        {
            // make sure all messages are not null and unique
            var all = l.OfType<TdApi.Message>().GroupBy(m => m.Id).Select(g => g.First()).OrderByDescending(m => m.Date).ToArray();

            var toBeReturned = all.Take(actualLimit);
            var toBeStacked  = all.Skip(actualLimit);

            // remember last message id
            foreach (var message in all.Reverse())
            {
                state.SetLastMessageId(message.ChatId, message.Id);
            }
            
            // put into stack
            foreach (var message in toBeStacked.Reverse())
            {
                state.PushMessageToStack(message.ChatId, message);
            }

            return toBeReturned;
        });
    }

    private IObservable<TdApi.Message> LoadChannelMessages(Chat chat, ChatLoadingState state, int limit, int offset)
    {
        if (chat.ChatData is not { } chatData) return Observable.Empty<TdApi.Message>();
        
        // get messages for corresponding chat
        return GetMessages(chatData, state.LastMessageId, limit, offset).Do(message =>
        {
            if (state.LastMessageId < message.Id)
            {
                state.LastMessageId = message.Id;
            }
        });
    }

    private IObservable<TdApi.Message> GetMessages(TdApi.Chat chat, long fromMessageId, int limit, int offset) => agent.Execute(new TdApi.GetChatHistory
    {
        ChatId        = chat.Id,
        FromMessageId = fromMessageId,
        Limit         = limit,
        Offset        = offset,
        OnlyLocal     = false
    }).SelectMany(history => history.Messages_);

    // ToDo: This method returns the most recent pin.  We may want to update to SearchChatMessages with a filter of Pinned so that we can find and display all of them?
    private IObservable<TdApi.Message> GetPinnedMessage(TdApi.Chat chat) => agent.Execute(new TdApi.GetChatPinnedMessage
    {
        ChatId = chat.Id
    });
}