using System.Reactive.Linq;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

public class PrevMessageLoader(MessageLoaderConductor conductor)
{
    private static readonly IChatLoader ChatLoader = Registry.Services.GetRequiredService<IChatLoader>();
    private static readonly IMessageLoader MessageLoader = Registry.Services.GetRequiredService<IMessageLoader>();
    private static readonly IMessageModelFactory MessageModelFactory = Registry.Services.GetRequiredService<IMessageModelFactory>();

    public IDisposable Bind(ExplorerViewModel viewModel, Chat chat) => viewModel
        .WhenAnyValue(m => m.VisibleRange)
        .Throttle(TimeSpan.FromSeconds(1))
        .Select(r => r.Index)
        .DistinctUntilChanged()
        .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
        .Where(index => index - 4 < 0) // top is within 4 items
        .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
        .Synchronize(conductor.Locker)
        .SelectSeq(_ => StartLoading(viewModel, chat))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Accept(list => HandleLoading(viewModel, chat, list));

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat)
    {
        conductor.IsBusy = true;

        var fromMessage = viewModel.SourceItems.Items
            .OfType<MessageModel>()
            .First()
            .Message;
            
        return LoadPrevMessages(chat, fromMessage)
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
        
    private static IObservable<IList<MessageModel>> LoadPrevMessages(Chat chat,  Message? fromMessage) => ChatLoader
        .LoadChat(chat.ChatData.Id)
        .SelectSeq(c => GetPrevMessages(c, fromMessage).Select(MessageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().ToList());

    private static IObservable<Message> GetPrevMessages(Chat chat, Message? fromMessage)
    {
        return MessageLoader.LoadPrevMessages(chat, fromMessage.MessageData.Id, 32);
    }
}