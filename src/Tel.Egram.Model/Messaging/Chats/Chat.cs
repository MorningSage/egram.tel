using TdLib;

namespace Tel.Egram.Model.Messaging.Chats;

public class Chat
{
    public TdApi.Chat ChatData { get; init; }

    public TdApi.User? User { get; init; }
}