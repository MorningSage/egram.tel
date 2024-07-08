using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Messaging.Mappers.VisualMessages;

public class VisualMessageModelFactory : IVisualMessageModelFactory
{
    public PhotoMessageModel MapToPhotoMessage(Message message, TdApi.MessageContent.MessagePhoto messagePhoto) => new()
    {
        PhotoData = messagePhoto.Photo,
        Text      = messagePhoto.Caption.Text
    };

    public StickerMessageModel MapToStickerMessage(Message message, TdApi.MessageContent.MessageSticker messageSticker) => new()
    {
        StickerData = messageSticker.Sticker
    };

    public VideoMessageModel MapToVideoMessage(Message message, TdApi.MessageContent.MessageVideo messageVideo) => new()
    {
        VideoData = messageVideo.Video,
        Caption   = messageVideo.Caption.Text
    };

    public UnsupportedMessageModel MapToExpiredPhotoMessage(Message message, TdApi.MessageContent.MessageExpiredPhoto expiredPhoto) => new() { Message = message };
    public UnsupportedMessageModel MapToAnimationMessage(Message message, TdApi.MessageContent.MessageAnimation messageAnimation) => new() { Message = message };
    public UnsupportedMessageModel MapToExpiredVideoMessage(Message message, TdApi.MessageContent.MessageExpiredVideo expiredVideo) => new() { Message = message };
    public UnsupportedMessageModel MapToVideoNoteMessage(Message message, TdApi.MessageContent.MessageVideoNote videoNote) => new() { Message = message };
}