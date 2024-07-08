using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

public class InitMessageLoader(MessageLoaderConductor conductor)
{
    private static readonly IChatLoader ChatLoader = Registry.Services.GetRequiredService<IChatLoader>();
    private static readonly IMessageLoader MessageLoader = Registry.Services.GetRequiredService<IMessageLoader>();
    private static readonly IMessageModelFactory MessageModelFactory = Registry.Services.GetRequiredService<IMessageModelFactory>();

    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat)
    {
        return viewModel.WhenAnyValue(m => m.VisibleRange)
            .Where(_ => viewModel.SourceItems.Count == 0) // only initial
            .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
            .Synchronize(conductor.Locker)
            .SelectSeq(_ => StartLoading(chat))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(list => HandleLoading(viewModel, chat, list));
    }

    private IObservable<IList<MessageModel>> StartLoading(Chat chat)
    {
        conductor.IsBusy = true;
            
        return LoadInitMessages(chat)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Finally(() => { conductor.IsBusy = false; });
    }

    private void HandleLoading(ExplorerViewModel viewModel, Chat chat, IList<MessageModel> messageModels)
    {
        // find last read message to scroll to it later
        var targetItem = messageModels.FirstOrDefault(m => m.Message.MessageData.Id == chat.ChatData.LastReadInboxMessageId);

        if (targetItem == null && messageModels.Count > 0)
        {
            targetItem = messageModels[messageModels.Count / 2];
        }

        viewModel.SourceItems.AddRange(messageModels);
        viewModel.TargetItem = targetItem;
    }
        
    private static IObservable<IList<MessageModel>> LoadInitMessages(Chat chat) => ChatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetInitMessages(c).Select(MessageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().ToList());

    private static IObservable<Message> GetInitMessages(Chat chat)
    {
        return MessageLoader.LoadInitMessages(chat, chat.ChatData.LastReadInboxMessageId, 32);
    }
}