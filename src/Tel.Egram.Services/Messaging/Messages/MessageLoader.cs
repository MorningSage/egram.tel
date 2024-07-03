using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Messages
{
    public class MessageLoader : IMessageLoader
    {
        private readonly IAgent _agent;
        private readonly int _limit = 10;

        public MessageLoader(IAgent agent)
        {
            _agent = agent;
        }

        public IObservable<Message> LoadMessage(long chatId, long messageId)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return scope.GetMessage(chatId, messageId)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        public IObservable<Message> LoadMessages(
            Aggregate feed,
            AggregateLoadingState state)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return LoadAggregateMessages(feed, state)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        public IObservable<Message> LoadInitMessages(
            Chat chat,
            long fromMessageId,
            int limit)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return GetMessages(chat.ChatData, fromMessageId, limit, -(limit - 1) / 2)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        public IObservable<Message> LoadPrevMessages(
            Chat chat,
            long fromMessageId,
            int limit)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return GetMessages(chat.ChatData, fromMessageId, limit, 0)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        public IObservable<Message> LoadNextMessages(
            Chat chat,
            long fromMessageId,
            int limit)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return GetMessages(chat.ChatData, fromMessageId, limit, -(limit - 1))
                .Where(m => m.Id != fromMessageId)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        public IObservable<Message> LoadPinnedMessage(Chat chat)
        {
            var scope = new MessageLoaderScope(_agent);
            
            return GetPinnedMessage(chat.ChatData)
                .Where(m => m != null)
                .SelectSeq(m => MapToMessage(scope, m))
                .Finally(() =>
                {
                    scope.Dispose();
                });
        }

        private IObservable<Message> MapToMessage(
            MessageLoaderScope scope,
            TdApi.Message msg,
            bool fetchReply = true)
        {
            return Observable.Return(new Message
                {
                    MessageData = msg
                })
                .SelectSeq(message =>
                {
                    // get chat data
                    return scope.GetChat(msg.ChatId)
                        .Select(chat =>
                        {
                            message.ChatData = chat;
                            return message;
                        });
                })
                .SelectSeq(message =>
                {
                    // get sender data
                    switch (message.MessageData.SenderId)
                    {
                        case TdApi.MessageSender.MessageSenderUser senderUser when senderUser.UserId != 0L:
                            return scope.GetUser(senderUser.UserId)
                                .Select(user =>
                                {
                                    message.UserData = user;
                                    return message;
                                });
                        case TdApi.MessageSender.MessageSenderChat senderChat when senderChat.ChatId != 0L:
                            return scope.GetChat(senderChat.ChatId)
                                .Select(chat =>
                                {
                                    // ToDo: "Senders" can be users or chats.  This needs to be updated to ensure everything still works
                                    message.UserData = null;
                                    return message;
                                });
                    }

                    return Observable.Return(message);
                })
                .SelectSeq(message =>
                {
                    // get reply data
                    switch (message.MessageData.ReplyTo)
                    {
                        case TdApi.MessageReplyTo.MessageReplyToMessage messageReplyToMessage when fetchReply:
                            return scope.GetMessage(messageReplyToMessage.ChatId, messageReplyToMessage.MessageId)
                                .SelectSeq(m => MapToMessage(scope, m, false))
                                .Select(reply =>
                                {
                                    message.ReplyMessage = reply;
                                    return message;
                                });
                        case TdApi.MessageReplyTo.MessageReplyToStory messageReplyToStory when fetchReply:
                            Debugger.Break();
                            
                            // ToDo: This may fail - are stories considered Messages?  Presumably not.
                            return scope.GetMessage(messageReplyToStory.StorySenderChatId, messageReplyToStory.StoryId)
                                .SelectSeq(m => MapToMessage(scope, m, false))
                                .Select(reply =>
                                {
                                    message.ReplyMessage = reply;
                                    return message;
                                });
                        default:
                            return Observable.Return(message);
                    }
                    
                    //if (fetchReply && message.MessageData.ReplyToMessageId != 0)
                    //{
                    //    return scope.GetMessage(message.MessageData.ChatId, message.MessageData.ReplyToMessageId)
                    //        .SelectSeq(m => MapToMessage(scope, m, false))
                    //        .Select(reply =>
                    //        {
                    //            message.ReplyMessage = reply;
                    //            return message;
                    //        });
                    //}
                    
                    return Observable.Return(message);
                });
        }

        private IObservable<TdApi.Message> LoadAggregateMessages(
            Aggregate aggregate,
            AggregateLoadingState state)
        {
            var actualLimit = _limit;
            
            var list = aggregate.Chats.Select(f =>
            {
                var stackedCount = state.CountStackedMessages(f.ChatData.Id);
                
                return Enumerable.Range(0, stackedCount)
                    .Select(_ => state.PopMessageFromStack(f.ChatData.Id)) // get stacked messages for this chat
                    .ToObservable()
                    .Concat(stackedCount < _limit
                        ? LoadChannelMessages(f, new ChatLoadingState // load messages from the server
                            {
                                LastMessageId = state.GetLastMessageId(f.ChatData.Id)
                            }, _limit, 0)
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
            
            return list.Merge()
                .CollectToList()
                .SelectMany(l =>
                {
                    // make sure all messages are unique
                    var all = l.GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .OrderByDescending(m => m.Date)
                        .ToArray();

                    var toBeReturned = all.Take(actualLimit);
                    var toBeStacked = all.Skip(actualLimit);

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

        private IObservable<TdApi.Message> LoadChannelMessages(
            Chat chat,
            ChatLoadingState state,
            int limit,
            int offset)
        {
            // get messages for corresponding chat
            return GetMessages(chat.ChatData, state.LastMessageId, limit, offset)
                .Do(message =>
                {
                    if (state.LastMessageId < message.Id)
                    {
                        state.LastMessageId = message.Id;
                    }
                });
        }

        private IObservable<TdApi.Message> GetMessages(
            TdApi.Chat chat,
            long fromMessageId,
            int limit,
            int offset)
        {   
            return _agent.Execute(new TdApi.GetChatHistory
                {
                    ChatId = chat.Id,
                    FromMessageId = fromMessageId,
                    Limit = limit,
                    Offset = offset,
                    OnlyLocal = false
                })
                .SelectMany(history => history.Messages_);
        }

        private IObservable<TdApi.Message> GetPinnedMessage(
            TdApi.Chat chat)
        {
            return _agent.Execute(new TdApi.GetChatPinnedMessage
            {
                ChatId = chat.Id
            });
        }
    }
}