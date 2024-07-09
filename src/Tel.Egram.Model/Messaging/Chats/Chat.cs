using TdLib;

namespace Tel.Egram.Model.Messaging.Chats;

public class Chat
{
    public required TdApi.Chat? ChatData { get; init; }

    public required TdApi.User? User { get; init; }
}