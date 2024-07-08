using TdLib;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Messages;

public class MessageSender(IAgent agent) : IMessageSender
{
    public IObservable<TdApi.Message> SendMessage(TdApi.Chat chat, TdApi.InputMessageContent.InputMessageText messageTextContent) => agent.Execute(new TdApi.SendMessage
    {
        ChatId = chat.Id,
        InputMessageContent = messageTextContent
    });
}