using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

public class PrevMessageLoader(MessageLoaderConductor conductor)
{
    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory) => viewModel
        .WhenAnyValue(m => m.VisibleRange)
        .Throttle(TimeSpan.FromSeconds(1))
        .Select(r => r.Index)
        .DistinctUntilChanged()
        .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
        .Where(index => index - 4 < 0) // top is within 4 items
        .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
        .Synchronize(conductor.Locker)
        .SelectSeq(_ => StartLoading(viewModel, chat, messageLoader, chatLoader, messageModelFactory))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Accept(list => HandleLoading(viewModel, chat, list));

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;

        var fromMessage = viewModel.SourceItems.Items
            .OfType<MessageModel>()
            .First()
            .Message;
            
        return LoadPrevMessages(chat, fromMessage, messageLoader, chatLoader, messageModelFactory)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Finally(() => { conductor.IsBusy = false; });
    }

    private static void HandleLoading(ExplorerViewModel viewModel, Chat chat, IList<MessageModel> messageModels)
    {
        var targetItem = default(ItemModel?);

        // find item which is currently visible to scroll to it later
        if (viewModel.SourceItems.Count > 0)
        {
            targetItem = viewModel.SourceItems.Items
                .Skip(viewModel.VisibleRange.Index)
                .OfType<MessageModel>()
                .FirstOrDefault();
        }
        
        // or take last added item
        else if (messageModels.Count > 0)
        {
            targetItem = messageModels[^1];
        }

        viewModel.TargetItem = targetItem;
        viewModel.SourceItems.InsertRange(messageModels, 0);
    }
        
    private static IObservable<IList<MessageModel>> LoadPrevMessages(Chat chat,  Message? fromMessage, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory) => chatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetPrevMessages(c, fromMessage, messageLoader).Select(messageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().ToList());

    private static IObservable<Message> GetPrevMessages(Chat chat, Message? fromMessage, IMessageLoader messageLoader)
    {
        return messageLoader.LoadPrevMessages(chat, fromMessage.MessageData.Id, 32);
    }
}