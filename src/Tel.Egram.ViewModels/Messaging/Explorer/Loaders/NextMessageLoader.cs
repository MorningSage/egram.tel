using System.ComponentModel;
using System.Reactive.Linq;
using DynamicData;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Messages;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Loaders;

public class NextMessageLoader(MessageLoaderConductor conductor)
{
    public void Bind(ExplorerViewModel viewModel, Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => viewModel.PropertyChanged += h, h => viewModel.PropertyChanged -= h)
            .Where(pattern => pattern.EventArgs.PropertyName is nameof(ExplorerViewModel.VisibleRange))
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => viewModel.VisibleRange.End)
            .DistinctUntilChanged()
            .Where(_ => viewModel.SourceItems.Count != 0) // skip initial
            .Where(index => index.Value + 4 > viewModel.SourceItems.Count) // bottom is within 4 items
            .Where(_ => !conductor.IsBusy) // ignore if other load are already in progress
            .Synchronize(conductor.Locker)
            .SelectSeq(_ => StartLoading(viewModel, chat, messageLoader, chatLoader, messageModelFactory))
            .Subscribe(list => viewModel.SourceItems.AddRange(list));
    }

    private IObservable<IList<MessageModel>> StartLoading(ExplorerViewModel viewModel, Chat chat, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory)
    {
        conductor.IsBusy = true;
        
        var fromMessage = viewModel.SourceItems.Items.OfType<MessageModel>().Last().Message;
            
        return LoadNextMessages(chat, fromMessage, messageLoader, chatLoader, messageModelFactory).Finally(() => conductor.IsBusy = false);
    }
    
    private static IObservable<IList<MessageModel>> LoadNextMessages(Chat chat, Message? fromMessage, IMessageLoader messageLoader, IChatLoader chatLoader, IMessageModelFactory messageModelFactory)
    {
        return chatLoader
            .LoadChat(chat.ChatData.Id)
            .SelectSeq(c => messageLoader.LoadNextMessages(c, fromMessage.MessageData.Id, 32).Select(messageModelFactory.MapToMessage).ToList())
            .Select(list => list.Reverse().Skip(1).ToList());
    }
}