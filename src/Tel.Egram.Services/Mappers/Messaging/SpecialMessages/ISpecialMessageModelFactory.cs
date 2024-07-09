using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Special;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Services.Mappers.Messaging.SpecialMessages;

public interface ISpecialMessageModelFactory
{
    DocumentMessageModel MapToDocumentMessage(Message message, TdApi.MessageContent.MessageDocument messageDocument);

    UnsupportedMessageModel MapToGameScoreMessage(Message message, TdApi.MessageContent.MessageGameScore gameScore);
    UnsupportedMessageModel MapToPaymentSuccessfulMessage(Message message, TdApi.MessageContent.MessagePaymentSuccessful paymentSuccessful);
    UnsupportedMessageModel MapToPaymentSuccessfulBotMessage(Message message, TdApi.MessageContent.MessagePaymentSuccessfulBot paymentSuccessfulBot);
    UnsupportedMessageModel MapToContactRegisteredMessage(Message message, TdApi.MessageContent.MessageContactRegistered contactRegistered);
    UnsupportedMessageModel MapToLocationMessage(Message message, TdApi.MessageContent.MessageLocation location);
    UnsupportedMessageModel MapToVenueMessage(Message message, TdApi.MessageContent.MessageVenue venue);
    UnsupportedMessageModel MapToContactMessage(Message message, TdApi.MessageContent.MessageContact contact);
    UnsupportedMessageModel MapToGameMessage(Message message, TdApi.MessageContent.MessageGame game);
    UnsupportedMessageModel MapToInvoiceMessage(Message message, TdApi.MessageContent.MessageInvoice invoice);
    UnsupportedMessageModel MapToPassportDataSentMessage(Message message, TdApi.MessageContent.MessagePassportDataSent passportDataSent);
    UnsupportedMessageModel MapToPassportDataReceivedMessage(Message message, TdApi.MessageContent.MessagePassportDataReceived passportDataReceived);
    UnsupportedMessageModel MapToAudioMessage(Message message, TdApi.MessageContent.MessageAudio messageAudio);
    UnsupportedMessageModel MapToVoiceNoteMessage(Message message, TdApi.MessageContent.MessageVoiceNote voiceNote);
}