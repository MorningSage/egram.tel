using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Model.Messaging.Explorer.Messages;

public abstract class MessageModel : ItemModel
{
    public string AuthorName { get; set; }

    public string Time { get; set; }
        
    public Avatar? Avatar { get; set; }
        
    public Message? Message { get; set; }
        
    // ToDo: Can this reference "Reply" and be automatic?  Why is this separate?
    public bool HasReply { get; set; }
        
    public ReplyModel? Reply { get; set; }
}