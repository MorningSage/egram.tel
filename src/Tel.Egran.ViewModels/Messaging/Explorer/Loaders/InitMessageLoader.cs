using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

public class InitMessageLoader(MessageLoaderConductor conductor)
{
    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        return viewModel.WhenAnyValue(m => m.VisibleRange)
            .Where(_ => viewModel.SourceItems.Count == 0) // only initial
            .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
            .Synchronize(conductor.Locker)
            .SelectSeq(_ => StartLoading(chat, chatLoader, messageLoader, messageModelFactory))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(list => HandleLoading(viewModel, chat, list));
    }

    private IObservable<IList<MessageModel>> StartLoading(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;
            
        return LoadInitMessages(chat, chatLoader, messageLoader, messageModelFactory)
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
        
    private static IObservable<IList<MessageModel>> LoadInitMessages(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory) => chatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetInitMessages(c, messageLoader).Select(messageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().ToList());

    private static IObservable<Message> GetInitMessages(Chat chat, IMessageLoader messageLoader)
    {
        return messageLoader.LoadInitMessages(chat, chat.ChatData.LastReadInboxMessageId, 32);
    }
}