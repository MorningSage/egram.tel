using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Special;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Utils.Formatting;

namespace Tel.Egram.Services.Messaging.Mappers.SpecialMessages;

public class SpecialMessageModelFactory(IStringFormatter stringFormatter) : ISpecialMessageModelFactory
{
    public DocumentMessageModel MapToDocumentMessage(Message message, TdApi.MessageContent.MessageDocument messageDocument) => new()
    {
        Document = messageDocument.Document,
        FileName = messageDocument.Document.FileName,
        Caption  = messageDocument.Caption.Text,
        Size     = $"({stringFormatter.FormatMemorySize(messageDocument.Document.Document_.Size)})"
    };

    public UnsupportedMessageModel MapToGameScoreMessage(Message message, TdApi.MessageContent.MessageGameScore gameScore) => new() { Message = message };
    public UnsupportedMessageModel MapToPaymentSuccessfulMessage(Message message, TdApi.MessageContent.MessagePaymentSuccessful paymentSuccessful) => new() { Message = message };
    public UnsupportedMessageModel MapToPaymentSuccessfulBotMessage(Message message, TdApi.MessageContent.MessagePaymentSuccessfulBot paymentSuccessfulBot) => new() { Message = message };
    public UnsupportedMessageModel MapToContactRegisteredMessage(Message message, TdApi.MessageContent.MessageContactRegistered contactRegistered) => new() { Message = message };
    public UnsupportedMessageModel MapToLocationMessage(Message message, TdApi.MessageContent.MessageLocation location) => new() { Message = message };
    public UnsupportedMessageModel MapToVenueMessage(Message message, TdApi.MessageContent.MessageVenue venue) => new() { Message = message };
    public UnsupportedMessageModel MapToContactMessage(Message message, TdApi.MessageContent.MessageContact contact) => new() { Message = message };
    public UnsupportedMessageModel MapToGameMessage(Message message, TdApi.MessageContent.MessageGame game) => new() { Message = message };
    public UnsupportedMessageModel MapToInvoiceMessage(Message message, TdApi.MessageContent.MessageInvoice invoice) => new() { Message = message };
    public UnsupportedMessageModel MapToPassportDataSentMessage(Message message, TdApi.MessageContent.MessagePassportDataSent passportDataSent) => new() { Message = message };
    public UnsupportedMessageModel MapToPassportDataReceivedMessage(Message message, TdApi.MessageContent.MessagePassportDataReceived passportDataReceived) => new() { Message = message };
    public UnsupportedMessageModel MapToAudioMessage(Message message, TdApi.MessageContent.MessageAudio messageAudio) => new() { Message = message };
    public UnsupportedMessageModel MapToVoiceNoteMessage(Message message, TdApi.MessageContent.MessageVoiceNote voiceNote) => new() { Message = message };
}