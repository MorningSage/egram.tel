using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public class StickerMessageModel : AbstractVisualMessageModel
{
    public override Preview? Preview { get; set; }
    public required TdApi.Sticker StickerData { get; init; }
}