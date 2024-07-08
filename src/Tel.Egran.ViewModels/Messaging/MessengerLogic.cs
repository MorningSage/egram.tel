using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Catalog;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;
using Tel.Egran.ViewModels.Messaging.Editor;
using Tel.Egran.ViewModels.Messaging.Explorer;
using Tel.Egran.ViewModels.Messaging.Explorer.Homepage;
using Tel.Egran.ViewModels.Messaging.Informer;

namespace Tel.Egran.ViewModels.Messaging;

public static class MessengerLogic
{
    public static IDisposable BindCatalog(this MessengerViewModel viewModel, Section section, IChatLoader chatLoader, IChatUpdater chatUpdater, IAvatarLoader avatarLoader)
    {
        viewModel.CatalogViewModel = new CatalogViewModel(section, chatLoader, chatUpdater, avatarLoader);
        return Disposable.Empty;
    }

    public static IDisposable BindInformer(this MessengerViewModel viewModel, IAvatarLoader avatarLoader)
    {
        viewModel.InformerViewModel = InformerViewModel.Hidden();

        return viewModel.SubscribeToSelection(entry => viewModel.InformerViewModel = entry switch
        {
            ChatEntryViewModel chatEntryModel           => new InformerViewModel(chatEntryModel.Chat, avatarLoader),
            AggregateEntryViewModel aggregateEntryModel => new InformerViewModel(aggregateEntryModel.Aggregate),
            HomeEntryViewModel                          => InformerViewModel.Hidden(),
            _                                       => viewModel.InformerViewModel
        });
    }

    public static IDisposable BindExplorer(this MessengerViewModel viewModel, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        viewModel.ExplorerViewModel = ExplorerViewModel.Hidden();
        
        return viewModel.SubscribeToSelection(entry => viewModel.ExplorerViewModel = entry switch
        {
            ChatEntryViewModel chatEntryModel => new ExplorerViewModel(chatEntryModel.Chat, chatLoader, messageLoader, messageModelFactory),
            AggregateEntryViewModel           => viewModel.ExplorerViewModel, //new ExplorerViewModel(aggregateEntryModel.Aggregate),
            HomeEntryViewModel                => ExplorerViewModel.Hidden(),
            _                             => viewModel.ExplorerViewModel // ToDo: viewModel.ExplorerViewModel or hidden?
        });
    }

    public static IDisposable BindHome(this MessengerViewModel viewModel)
    {
        viewModel.HomepageViewModel = HomepageViewModel.Hidden();

        return viewModel.SubscribeToSelection(entry => viewModel.HomepageViewModel = entry switch
        {
            HomeEntryViewModel => new HomepageViewModel(),
            _              => HomepageViewModel.Hidden() // ToDo: viewModel.ExplorerViewModel or hidden?
        });
    }

    public static IDisposable BindEditor(this MessengerViewModel viewModel, IMessageSender messageSender)
    {
        viewModel.EditorModel = EditorViewModel.Hidden();

        return viewModel.SubscribeToSelection(entry => viewModel.EditorModel = entry switch
        {
            ChatEntryViewModel chatEntryModel => new EditorViewModel(chatEntryModel.Chat, messageSender),
            _                             => EditorViewModel.Hidden() // ToDo: viewModel.ExplorerViewModel or hidden?
        });
    }

    private static IDisposable SubscribeToSelection(this MessengerViewModel viewModel, Action<EntryViewModel> action)
    {
        return viewModel.WhenAnyValue(ctx => ctx.CatalogViewModel.SelectedEntry)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(entry => { if (entry != null) action(entry); });
    }
}