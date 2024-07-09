using Tel.Egram.Model.Graphics.Avatars;

namespace Tel.Egram.Model.Messaging.Explorer.Messages;

public class MessageAuthorModel
{
    public required string Name { get; set; }
    public required Avatar Avatar { get; set; }
}