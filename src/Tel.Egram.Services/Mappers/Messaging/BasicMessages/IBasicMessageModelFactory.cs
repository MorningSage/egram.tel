using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Basic;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.BasicMessages;

public interface IBasicMessageModelFactory
{
    BasicMessageModel MapToTextMessage(Message message, TdApi.MessageContent.MessageText messageText);

    UnsupportedMessageModel MapToUnsupportedMessage(Message message);
}