using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Editor;

public static class SenderLogic
{
    private static readonly IMessageSender MessageSender = Registry.Services.GetRequiredService<IMessageSender>();
    
    public static IDisposable BindSender(this EditorViewModel viewModel, Chat chat)
    {
        var canSendCode = viewModel.WhenAnyValue(m => m.Text).Select(text => !string.IsNullOrWhiteSpace(text));

        viewModel.SendCommand = ReactiveCommand.CreateFromObservable(
            () => MessageSender.SendMessage(chat.ChatData, new TdApi.InputMessageContent.InputMessageText
            {
                ClearDraft = true,
                Text = new TdApi.FormattedText { Text = viewModel.Text }
            }).Select(_ => Unit.Default),
            canSendCode,
            RxApp.MainThreadScheduler
        );
        
        return viewModel.SendCommand
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(_ =>
            {
                viewModel.Text = null;
            });
    }
}