using System.ComponentModel;
using System.Reactive.Linq;
using DynamicData;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Loaders;

public class InitMessageLoader(MessageLoaderConductor conductor)
{
    public void Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => viewModel.PropertyChanged += h, h => viewModel.PropertyChanged -= h)
            .Where(pattern => pattern.EventArgs.PropertyName is nameof(ExplorerViewModel.VisibleRange))
            .Where(_ => viewModel.SourceItems.Count is 0) // only initial
            .Where(_ => !conductor.IsBusy)// ignore if other load are already in progress
            .Synchronize(conductor.Locker)
            .SelectMany(_ => StartLoading(chat, chatLoader, messageLoader, messageModelFactory))
            .Subscribe(list => HandleLoading(viewModel, chat, list));
    }

    private IObservable<IList<MessageModel>> StartLoading(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;
        return LoadInitMessages(chat, chatLoader, messageLoader, messageModelFactory).Finally(() => conductor.IsBusy = false);
    }

    private static void HandleLoading(ExplorerViewModel viewModel, Chat chat, IList<MessageModel> messageModels)
    {
        // find last read message to scroll to it later
        var targetItem = messageModels.FirstOrDefault(m => m.Message?.MessageData.Id == chat.ChatData?.LastReadInboxMessageId);

        if (targetItem == null && messageModels.Count > 0)
        {
            targetItem = messageModels[messageModels.Count / 2];
        }

        viewModel.SourceItems.AddRange(messageModels);
        viewModel.TargetItem = targetItem;
    }

    private static IObservable<IList<MessageModel>> LoadInitMessages(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        return chatLoader
            .LoadChat(chat.ChatData.Id)
            .SelectSeq(c => messageLoader.LoadInitMessages(c, chat.ChatData.LastReadInboxMessageId, 32).Select(messageModelFactory.MapToMessage).ToList())
            .Select(list => list.Reverse().ToList());
    }
}