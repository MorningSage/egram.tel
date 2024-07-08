using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Messaging.Mappers;

public interface IMessageModelFactory
{
    MessageModel MapToMessage(Message message);
}