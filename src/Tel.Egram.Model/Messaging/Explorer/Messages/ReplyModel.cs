using PropertyChanged;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Model.Messaging.Explorer.Messages;

[AddINotifyPropertyChangedInterface]
public class ReplyModel
{
    public string AuthorName { get; set; }
        
    public string? Text { get; set; }
        
    public bool HasPreview { get; set; }
        
    public Preview? Preview { get; set; }
        
    public Message Message { get; set; }
        
    public TdApi.Photo? PhotoData { get; set; }
        
    public TdApi.Sticker? StickerData { get; set; }
        
    public TdApi.Video? VideoData { get; set; }
}