using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public class VideoMessageModel : AbstractVisualMessageModel
{
    public override Preview? Preview { get; set; }
    public required TdApi.Video VideoData { get; init; }
    public required string Caption { get; init; }
}