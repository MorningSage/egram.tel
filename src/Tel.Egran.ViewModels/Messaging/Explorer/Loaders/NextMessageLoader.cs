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

public class NextMessageLoader(MessageLoaderConductor conductor)
{
    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory) => viewModel
        .WhenAnyValue(m => m.VisibleRange)
        .Throttle(TimeSpan.FromSeconds(1))
        .Select(r => r.LastIndex)
        .DistinctUntilChanged()
        .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
        .Where(index => index + 4 > viewModel.SourceItems.Count) // bottom is within 4 items
        .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
        .Synchronize(conductor.Locker)
        .SelectSeq(_ => StartLoading(viewModel, chat, messageLoader, chatLoader, messageModelFactory))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Accept(list => HandleLoading(viewModel, list));

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;

        var fromMessage = viewModel.SourceItems.Items
            .OfType<MessageModel>()
            .Last()
            .Message;
            
        return LoadNextMessages(chat, fromMessage, messageLoader, chatLoader, messageModelFactory)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Finally(() => { conductor.IsBusy = false; });
    }

    private static void HandleLoading(ExplorerViewModel viewModel, IList<MessageModel> messageModels)
    {
        viewModel.SourceItems.AddRange(messageModels);
    }
        
    private IObservable<IList<MessageModel>> LoadNextMessages(Chat chat, Message fromMessage, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory) => chatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetNextMessages(c, fromMessage, messageLoader).Select(messageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().Skip(1).ToList());

    private static IObservable<Message> GetNextMessages(Chat chat, Message fromMessage, IMessageLoader messageLoader)
    {
        return messageLoader.LoadNextMessages(chat, fromMessage.MessageData.Id, 32);
    }
}