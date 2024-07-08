using TdLib;

namespace Tel.Egram.Model.Messaging.Messages;

public class Message
{
    public Message? ReplyMessage { get; set; }
        
    public TdApi.Message MessageData { get; set; }
        
    public TdApi.Chat ChatData { get; set; }
        
    public TdApi.User? UserData { get; set; }
}