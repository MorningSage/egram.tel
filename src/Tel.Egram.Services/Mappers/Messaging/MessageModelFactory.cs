using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Mappers.Messaging.BasicMessages;
using Tel.Egram.Services.Mappers.Messaging.NoteMessages;
using Tel.Egram.Services.Mappers.Messaging.SpecialMessages;
using Tel.Egram.Services.Mappers.Messaging.VisualMessages;
using Tel.Egram.Services.Utils.Formatting;

namespace Tel.Egram.Services.Mappers.Messaging;

public class MessageModelFactory(IBasicMessageModelFactory basicMessageModelFactory, INoteMessageModelFactory noteMessageModelFactory, ISpecialMessageModelFactory specialMessageModelFactory, IVisualMessageModelFactory visualMessageModelFactory, IStringFormatter stringFormatter) : IMessageModelFactory
{
    public MessageModel MapToMessage(Message message)
    {
        var model = MapToMessageModel(message);
        ApplyMessageAttributes(model, message);
        return model;
    }

    private MessageModel MapToMessageModel(Message message)
    {
        return message.MessageData.Content switch
        {
            // basic
            TdApi.MessageContent.MessageText messageText => basicMessageModelFactory.MapToTextMessage(message, messageText),
            
            // visual
            TdApi.MessageContent.MessagePhoto messagePhoto         => visualMessageModelFactory.MapToPhotoMessage(message, messagePhoto),
            TdApi.MessageContent.MessageExpiredPhoto expiredPhoto  => visualMessageModelFactory.MapToExpiredPhotoMessage(message, expiredPhoto),
            TdApi.MessageContent.MessageSticker messageSticker     => visualMessageModelFactory.MapToStickerMessage(message, messageSticker),
            TdApi.MessageContent.MessageAnimation messageAnimation => visualMessageModelFactory.MapToAnimationMessage(message, messageAnimation),
            TdApi.MessageContent.MessageVideo messageVideo         => visualMessageModelFactory.MapToVideoMessage(message, messageVideo),
            TdApi.MessageContent.MessageExpiredVideo expiredVideo  => visualMessageModelFactory.MapToExpiredVideoMessage(message, expiredVideo),
            TdApi.MessageContent.MessageVideoNote videoNote        => visualMessageModelFactory.MapToVideoNoteMessage(message, videoNote),
            
            // special
            TdApi.MessageContent.MessageDocument messageDocument                  => specialMessageModelFactory.MapToDocumentMessage(message, messageDocument),
            TdApi.MessageContent.MessageAudio messageAudio                        => specialMessageModelFactory.MapToAudioMessage(message, messageAudio),
            TdApi.MessageContent.MessageVoiceNote voiceNote                       => specialMessageModelFactory.MapToVoiceNoteMessage(message, voiceNote),
            TdApi.MessageContent.MessagePaymentSuccessful paymentSuccessful       => specialMessageModelFactory.MapToPaymentSuccessfulMessage(message, paymentSuccessful),
            TdApi.MessageContent.MessagePaymentSuccessfulBot paymentSuccessfulBot => specialMessageModelFactory.MapToPaymentSuccessfulBotMessage(message, paymentSuccessfulBot),
            TdApi.MessageContent.MessageLocation location                         => specialMessageModelFactory.MapToLocationMessage(message, location),
            TdApi.MessageContent.MessageVenue venue                               => specialMessageModelFactory.MapToVenueMessage(message, venue),
            TdApi.MessageContent.MessageContact contact                           => specialMessageModelFactory.MapToContactMessage(message, contact),
            TdApi.MessageContent.MessageGame game                                 => specialMessageModelFactory.MapToGameMessage(message, game),
            TdApi.MessageContent.MessageGameScore gameScore                       => specialMessageModelFactory.MapToGameScoreMessage(message, gameScore),
            TdApi.MessageContent.MessageInvoice invoice                           => specialMessageModelFactory.MapToInvoiceMessage(message, invoice),
            TdApi.MessageContent.MessagePassportDataSent passportDataSent         => specialMessageModelFactory.MapToPassportDataSentMessage(message, passportDataSent),
            TdApi.MessageContent.MessagePassportDataReceived passportDataReceived => specialMessageModelFactory.MapToPassportDataReceivedMessage(message, passportDataReceived),
            TdApi.MessageContent.MessageContactRegistered contactRegistered       => specialMessageModelFactory.MapToContactRegisteredMessage(message, contactRegistered),
            
            // notes
            TdApi.MessageContent.MessageCall messageCall                          => noteMessageModelFactory.MapToCallMessage(message, messageCall),
            TdApi.MessageContent.MessageBasicGroupChatCreate basicGroupChatCreate => noteMessageModelFactory.MapToBasicGroupChatCreateMessage(message, basicGroupChatCreate),
            TdApi.MessageContent.MessageChatChangeTitle chatChangeTitle           => noteMessageModelFactory.MapToChatChangeTitleMessage(message, chatChangeTitle),
            TdApi.MessageContent.MessageChatChangePhoto chatChangePhoto           => noteMessageModelFactory.MapToChatChangePhotoMessage(message, chatChangePhoto),
            TdApi.MessageContent.MessageChatDeletePhoto chatDeletePhoto           => noteMessageModelFactory.MapToChatDeletePhotoMessage(message, chatDeletePhoto),
            TdApi.MessageContent.MessageChatAddMembers chatAddMembers             => noteMessageModelFactory.MapToChatAddMembersMessage(message, chatAddMembers),
            TdApi.MessageContent.MessageChatJoinByLink chatJoinByLink             => noteMessageModelFactory.MapToChatJoinByLinkMessage(message, chatJoinByLink),
            TdApi.MessageContent.MessageChatDeleteMember chatDeleteMember         => noteMessageModelFactory.MapToChatDeleteMemberMessage(message, chatDeleteMember),
            TdApi.MessageContent.MessageChatUpgradeTo chatUpgradeTo               => noteMessageModelFactory.MapToChatUpgradeToMessage(message, chatUpgradeTo),
            TdApi.MessageContent.MessageChatUpgradeFrom chatUpgradeFrom           => noteMessageModelFactory.MapToChatUpgradeFromMessage(message, chatUpgradeFrom),
            TdApi.MessageContent.MessagePinMessage pinMessage                     => noteMessageModelFactory.MapToPinMessageMessage(message, pinMessage),
            TdApi.MessageContent.MessageScreenshotTaken screenshotTaken           => noteMessageModelFactory.MapToScreenshotTakenMessage(message, screenshotTaken),
            TdApi.MessageContent.MessageCustomServiceAction customServiceAction   => noteMessageModelFactory.MapToCustomServiceActionMessage(message, customServiceAction),
            
            // ToDo: Not yet implemented, these are new since updating
            TdApi.MessageContent.MessageAnimatedEmoji messageAnimatedEmoji                               => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageBotWriteAccessAllowed messageBotWriteAccessAllowed               => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatBoost messageChatBoost                                       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatJoinByRequest messageChatJoinByRequest                       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatSetBackground messageChatSetBackground                       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatSetMessageAutoDeleteTime messageChatSetMessageAutoDeleteTime => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatSetTheme messageChatSetTheme                                 => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageChatShared messageChatShared                                     => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageDice messageDice                                                 => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageExpiredVideoNote messageExpiredVideoNote                         => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageExpiredVoiceNote messageExpiredVoiceNote                         => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageForumTopicCreated messageForumTopicCreated                       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageForumTopicEdited messageForumTopicEdited                         => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageForumTopicIsClosedToggled messageForumTopicIsClosedToggled       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageForumTopicIsHiddenToggled messageForumTopicIsHiddenToggled       => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageGiftedPremium messageGiftedPremium                               => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageInviteVideoChatParticipants messageInviteVideoChatParticipants   => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePoll messagePoll                                                 => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePremiumGiftCode messagePremiumGiftCode                           => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePremiumGiveaway messagePremiumGiveaway                           => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePremiumGiveawayCompleted messagePremiumGiveawayCompleted         => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePremiumGiveawayCreated messagePremiumGiveawayCreated             => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessagePremiumGiveawayWinners messagePremiumGiveawayWinners             => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageProximityAlertTriggered messageProximityAlertTriggered           => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageStory messageStory                                               => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageSuggestProfilePhoto messageSuggestProfilePhoto                   => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageSupergroupChatCreate messageSupergroupChatCreate                 => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageUsersShared messageUsersShared                                   => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageVideoChatEnded messageVideoChatEnded                             => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageVideoChatScheduled messageVideoChatScheduled                     => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageVideoChatStarted messageVideoChatStarted                         => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageWebAppDataReceived messageWebAppDataReceived                     => basicMessageModelFactory.MapToUnsupportedMessage(message),
            TdApi.MessageContent.MessageWebAppDataSent messageWebAppDataSent                             => basicMessageModelFactory.MapToUnsupportedMessage(message),
            
            // unsupported
            TdApi.MessageContent.MessageUnsupported messageUnsupported => basicMessageModelFactory.MapToUnsupportedMessage(message),
            _ => basicMessageModelFactory.MapToUnsupportedMessage(message)

        };
    }

    private void ApplyMessageAttributes(MessageModel model, Message message)
    {
        var user = message.UserData;
        
        var authorName = user == null
            ? message.ChatData.Title
            : $"{user.FirstName} {user.LastName}";
                
        model.Message = message;
        model.AuthorName = authorName;
        model.Time = stringFormatter.FormatShortTime(message.MessageData.Date);

        if (message.ReplyMessage == null) return;
        
        model.HasReply = true;
        model.Reply = new ReplyModel
        {
            Message     = message.ReplyMessage,
            AuthorName  = GetReplyAuthorName(message.ReplyMessage),
            Text        = GetReplyText(message.ReplyMessage),
            PhotoData   = GetReplyPhoto(message.ReplyMessage),
            VideoData   = GetReplyVideo(message.ReplyMessage),
            StickerData = GetReplySticker(message.ReplyMessage)
        };
    }

    private static string GetReplyAuthorName(Message message)
    {
        // ToDo: Trailing space if the user has no last name
        if (message.UserData is { } replyUser) return $"{replyUser.FirstName} {replyUser.LastName}";
        return message.ChatData.Title;
    }

    private static string? GetReplyText(Message message)
    {
        var text = message.MessageData.Content switch
        {
            TdApi.MessageContent.MessageText messageText => messageText.Text?.Text,
            TdApi.MessageContent.MessagePhoto messagePhoto => messagePhoto.Caption?.Text,
            _ => null
        };

        return text != null
            ? new string(text.Take(64).TakeWhile(c => c != '\n' && c != '\r').ToArray())
            : text;
    }

    private static TdApi.Photo? GetReplyPhoto(Message message)
    {
        if (message.MessageData.Content is TdApi.MessageContent.MessagePhoto messagePhoto) return messagePhoto.Photo;
        return null;
    }

    private static TdApi.Video? GetReplyVideo(Message message)
    {
        if (message.MessageData.Content is TdApi.MessageContent.MessageVideo messageVideo) return messageVideo.Video;
        return null;
    }

    private TdApi.Sticker? GetReplySticker(Message message)
    {
        if (message.MessageData.Content is TdApi.MessageContent.MessageSticker messageSticker) return messageSticker.Sticker;
        return null;
    }
}