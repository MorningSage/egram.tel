using System.ComponentModel;
using System.Reactive.Linq;
using DynamicData;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

public class PrevMessageLoader(MessageLoaderConductor conductor)
{
    public void Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => viewModel.PropertyChanged += h, h => viewModel.PropertyChanged -= h)
            .Where(pattern => pattern.EventArgs.PropertyName is nameof(ExplorerViewModel.VisibleRange))
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => viewModel.VisibleRange.Start)
            .DistinctUntilChanged()
            .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
            .Where(index => index.Value - 4 < 0) // top is within 4 items
            .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
            .Synchronize(conductor.Locker)
            .SelectSeq(_ => StartLoading(viewModel, chat, messageLoader, chatLoader, messageModelFactory))
            .Subscribe(list => HandleLoading(viewModel, chat, list));
    }

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;

        var fromMessage = viewModel.SourceItems.Items.OfType<MessageModel>().First().Message;
            
        return LoadPrevMessages(chat, fromMessage, messageLoader, chatLoader, messageModelFactory).Finally(() => conductor.IsBusy = false);
    }

    private static void HandleLoading(ExplorerViewModel viewModel, Chat chat, IList<MessageModel> messageModels)
    {
        var targetItem = default(ItemModel?);

        // find item which is currently visible to scroll to it later
        if (viewModel.SourceItems.Count > 0)
        {
            targetItem = viewModel.SourceItems.Items
                .Skip(viewModel.VisibleRange.Start.Value)
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
        .SelectSeq(c => messageLoader.LoadPrevMessages(c, fromMessage.MessageData.Id, 32).Select(messageModelFactory.MapToMessage).ToList())
        .Select(list => list.Reverse().ToList());
}