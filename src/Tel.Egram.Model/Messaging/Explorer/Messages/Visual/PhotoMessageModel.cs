using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public class PhotoMessageModel : AbstractVisualMessageModel
{
    public override Preview? Preview { get; set; }
    public required TdApi.Photo PhotoData { get; init; }
    public required string Text { get; init; }
}