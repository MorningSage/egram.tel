using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.VisualMessages;

public interface IVisualMessageModelFactory
{
    PhotoMessageModel MapToPhotoMessage(Message message, TdApi.MessageContent.MessagePhoto messagePhoto);
    StickerMessageModel MapToStickerMessage(Message message, TdApi.MessageContent.MessageSticker messageSticker);
    VideoMessageModel MapToVideoMessage(Message message, TdApi.MessageContent.MessageVideo messageVideo);
    UnsupportedMessageModel MapToExpiredPhotoMessage(Message message, TdApi.MessageContent.MessageExpiredPhoto expiredPhoto);
    UnsupportedMessageModel MapToAnimationMessage(Message message, TdApi.MessageContent.MessageAnimation messageAnimation);
    UnsupportedMessageModel MapToExpiredVideoMessage(Message message, TdApi.MessageContent.MessageExpiredVideo expiredVideo);
    UnsupportedMessageModel MapToVideoNoteMessage(Message message, TdApi.MessageContent.MessageVideoNote videoNote);
}