using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Services.Mappers.Messaging;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egran.ViewModels.Messaging.Explorer.Loaders;

namespace Tel.Egran.ViewModels.Messaging.Explorer;

public partial class ExplorerViewModel : AbstractViewModelBase
{
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private Range _visibleRange;
    [ObservableProperty] private ItemModel? _targetItem;
    [ObservableProperty] private ObservableCollectionExtended<ItemModel> _items = [];
    [ObservableProperty] private SourceList<ItemModel> _sourceItems = new();

    public ExplorerViewModel(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        SourceItems.Connect().Bind(Items).Subscribe();

        var conductor = new MessageLoaderConductor();

        new InitMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory);
        new NextMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory);
        new PrevMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory);
    }
    
    private ExplorerViewModel() { }
    
    public static ExplorerViewModel Hidden() => new() { IsVisible = false };
}