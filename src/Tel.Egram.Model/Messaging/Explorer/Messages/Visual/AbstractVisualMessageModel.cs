namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public abstract class AbstractVisualMessageModel : MessageModel
{
    public abstract Preview? Preview { get; set; }
}