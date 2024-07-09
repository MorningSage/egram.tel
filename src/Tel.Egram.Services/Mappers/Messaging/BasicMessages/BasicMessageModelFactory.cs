using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Basic;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.BasicMessages;

public class BasicMessageModelFactory : IBasicMessageModelFactory
{
    public BasicMessageModel MapToTextMessage(Message message, TdApi.MessageContent.MessageText messageText) => new()
    {
        Text = messageText.Text.Text
    };

    public UnsupportedMessageModel MapToUnsupportedMessage(Message message) => new()
    {
        Message = message
    };
}