using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.NoteMessages;

public interface INoteMessageModelFactory
{
    UnsupportedMessageModel MapToCallMessage(Message message, TdApi.MessageContent.MessageCall messageCall);
    UnsupportedMessageModel MapToBasicGroupChatCreateMessage(Message message, TdApi.MessageContent.MessageBasicGroupChatCreate basicGroupChatCreate);
    UnsupportedMessageModel MapToChatChangeTitleMessage(Message message, TdApi.MessageContent.MessageChatChangeTitle chatChangeTitle);
    UnsupportedMessageModel MapToChatChangePhotoMessage(Message message, TdApi.MessageContent.MessageChatChangePhoto chatChangePhoto);
    UnsupportedMessageModel MapToChatDeletePhotoMessage(Message message, TdApi.MessageContent.MessageChatDeletePhoto chatDeletePhoto);
    UnsupportedMessageModel MapToChatAddMembersMessage(Message message, TdApi.MessageContent.MessageChatAddMembers chatAddMembers);
    UnsupportedMessageModel MapToChatJoinByLinkMessage(Message message, TdApi.MessageContent.MessageChatJoinByLink chatJoinByLink);
    UnsupportedMessageModel MapToChatDeleteMemberMessage(Message message, TdApi.MessageContent.MessageChatDeleteMember chatDeleteMember);
    UnsupportedMessageModel MapToChatUpgradeToMessage(Message message, TdApi.MessageContent.MessageChatUpgradeTo chatUpgradeTo);
    UnsupportedMessageModel MapToChatUpgradeFromMessage(Message message, TdApi.MessageContent.MessageChatUpgradeFrom chatUpgradeFrom);
    UnsupportedMessageModel MapToPinMessageMessage(Message message, TdApi.MessageContent.MessagePinMessage pinMessage);
    UnsupportedMessageModel MapToScreenshotTakenMessage(Message message, TdApi.MessageContent.MessageScreenshotTaken screenshotTaken);
    UnsupportedMessageModel MapToCustomServiceActionMessage(Message message, TdApi.MessageContent.MessageCustomServiceAction customServiceAction);
}