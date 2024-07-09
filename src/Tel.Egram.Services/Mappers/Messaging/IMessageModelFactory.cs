using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging;

public interface IMessageModelFactory
{
    MessageModel MapToMessage(Message message);
}