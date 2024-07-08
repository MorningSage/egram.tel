using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Explorer.Loaders;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Explorer.Loaders;
using Range = Tel.Egram.Services.Utils.Range;

namespace Tel.Egran.ViewModels.Messaging.Explorer;

[AddINotifyPropertyChangedInterface]
public class ExplorerViewModel : IActivatableViewModel
{
    public bool IsVisible { get; set; } = true;
    public Range VisibleRange { get; set; }
    public ItemModel? TargetItem { get; set; }
    public ObservableCollectionExtended<ItemModel> Items { get; set; } = [];
    public SourceList<ItemModel> SourceItems { get; set; } = new();

    public ExplorerViewModel(Chat chat, IChatLoader chatLoader, IMessageLoader messageLoader, IMessageModelFactory messageModelFactory)
    {
        this.WhenActivated(disposables =>
        {
            BindSource().DisposeWith(disposables);

            var conductor = new MessageLoaderConductor();
                
            new InitMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory).DisposeWith(disposables);
            new NextMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory).DisposeWith(disposables);
            new PrevMessageLoader(conductor).Bind(this, chat, chatLoader, messageLoader, messageModelFactory).DisposeWith(disposables);
        });
    }

    private IDisposable BindSource() => SourceItems.Connect().Bind(Items).Accept();

    private ExplorerViewModel() { }
        
    public static ExplorerViewModel Hidden()
    {
        return new ExplorerViewModel { IsVisible = false };
    }
        
    public ViewModelActivator Activator { get; } = new();
}