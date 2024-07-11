using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;

namespace Tel.Egram.ViewModels.Messaging.Editor;

public partial class EditorViewModel : AbstractViewModelBase
{
    private readonly Chat _chat;
    private readonly IMessageSender _messageSender;

    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private string? _text;

    private bool CanSendMessage => !string.IsNullOrWhiteSpace(Text);
    
    public EditorViewModel(Chat chat, IMessageSender messageSender)
    {
        _chat = chat;
        _messageSender = messageSender;
    }

    private EditorViewModel() { }
    
    public static EditorViewModel Hidden() => new() { IsVisible = false };
    
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private void SendMessage()
    {
        _messageSender.SendMessage(_chat.ChatData, new TdApi.InputMessageContent.InputMessageText
        {
            ClearDraft = true,
            Text = new TdApi.FormattedText { Text = Text }
        });
        
        Text = null;
    }
}