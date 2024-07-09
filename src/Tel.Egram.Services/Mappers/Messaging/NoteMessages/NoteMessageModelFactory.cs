using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.NoteMessages;

public class NoteMessageModelFactory : INoteMessageModelFactory
{
    public UnsupportedMessageModel MapToCallMessage(Message message, TdApi.MessageContent.MessageCall messageCall) => new() { Message = message };
    public UnsupportedMessageModel MapToBasicGroupChatCreateMessage(Message message, TdApi.MessageContent.MessageBasicGroupChatCreate basicGroupChatCreate) => new() { Message = message };
    public UnsupportedMessageModel MapToChatChangeTitleMessage(Message message, TdApi.MessageContent.MessageChatChangeTitle chatChangeTitle) => new() { Message = message };
    public UnsupportedMessageModel MapToChatChangePhotoMessage(Message message, TdApi.MessageContent.MessageChatChangePhoto chatChangePhoto) => new() { Message = message };
    public UnsupportedMessageModel MapToChatDeletePhotoMessage(Message message, TdApi.MessageContent.MessageChatDeletePhoto chatDeletePhoto) => new() { Message = message };
    public UnsupportedMessageModel MapToChatAddMembersMessage(Message message, TdApi.MessageContent.MessageChatAddMembers chatAddMembers) => new() { Message = message };
    public UnsupportedMessageModel MapToChatJoinByLinkMessage(Message message, TdApi.MessageContent.MessageChatJoinByLink chatJoinByLink) => new() { Message = message };
    public UnsupportedMessageModel MapToChatDeleteMemberMessage(Message message, TdApi.MessageContent.MessageChatDeleteMember chatDeleteMember) => new() { Message = message };
    public UnsupportedMessageModel MapToChatUpgradeToMessage(Message message, TdApi.MessageContent.MessageChatUpgradeTo chatUpgradeTo) => new() { Message = message };
    public UnsupportedMessageModel MapToChatUpgradeFromMessage(Message message, TdApi.MessageContent.MessageChatUpgradeFrom chatUpgradeFrom) => new() { Message = message };
    public UnsupportedMessageModel MapToPinMessageMessage(Message message, TdApi.MessageContent.MessagePinMessage pinMessage) => new() { Message = message };
    public UnsupportedMessageModel MapToScreenshotTakenMessage(Message message, TdApi.MessageContent.MessageScreenshotTaken screenshotTaken) => new() { Message = message };
    public UnsupportedMessageModel MapToCustomServiceActionMessage(Message message, TdApi.MessageContent.MessageCustomServiceAction customServiceAction) => new() { Message = message };
}