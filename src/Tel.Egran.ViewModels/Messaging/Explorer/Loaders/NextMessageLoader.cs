using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
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

public class NextMessageLoader(MessageLoaderConductor conductor)
{
    private static readonly IChatLoader ChatLoader = Registry.Services.GetRequiredService<IChatLoader>();
    private static readonly IMessageLoader MessageLoader = Registry.Services.GetRequiredService<IMessageLoader>();
    private static readonly IMessageModelFactory MessageModelFactory = Registry.Services.GetRequiredService<IMessageModelFactory>();
    
    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat) => viewModel
        .WhenAnyValue(m => m.VisibleRange)
        .Throttle(TimeSpan.FromSeconds(1))
        .Select(r => r.LastIndex)
        .DistinctUntilChanged()
        .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
        .Where(index => index + 4 > viewModel.SourceItems.Count) // bottom is within 4 items
        .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
        .Synchronize(conductor.Locker)
        .SelectSeq(_ => StartLoading(viewModel, chat))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Accept(list => HandleLoading(viewModel, list));

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat)
    {
        conductor.IsBusy = true;

        var fromMessage = viewModel.SourceItems.Items
            .OfType<MessageModel>()
            .Last()
            .Message;
            
        return LoadNextMessages(chat, fromMessage)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SubscribeOn(RxApp.MainThreadScheduler)
            .Finally(() => { conductor.IsBusy = false; });
    }

    private static void HandleLoading(ExplorerViewModel viewModel, IList<MessageModel> messageModels)
    {
        viewModel.SourceItems.AddRange(messageModels);
    }
        
    private IObservable<IList<MessageModel>> LoadNextMessages(Chat chat, Message fromMessage) => ChatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetNextMessages(c, fromMessage).Select(MessageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().Skip(1).ToList());

    private static IObservable<Message> GetNextMessages(Chat chat, Message fromMessage)
    {
        return MessageLoader.LoadNextMessages(chat, fromMessage.MessageData.Id, 32);
    }
}