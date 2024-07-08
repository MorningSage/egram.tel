using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egran.ViewModels.Messaging.Catalog;
using Tel.Egran.ViewModels.Messaging.Editor;
using Tel.Egran.ViewModels.Messaging.Explorer;
using Tel.Egran.ViewModels.Messaging.Explorer.Homepage;
using Tel.Egran.ViewModels.Messaging.Informer;

namespace Tel.Egran.ViewModels.Messaging;

[AddINotifyPropertyChangedInterface]
public class MessengerViewModel : IActivatableViewModel
{   
    public CatalogViewModel CatalogViewModel { get; set; }
        
    public InformerViewModel InformerViewModel { get; set; }
        
    public ExplorerViewModel ExplorerViewModel { get; set; }
        
    public HomepageViewModel HomepageViewModel { get; set; }
        
    public EditorViewModel EditorModel { get; set; }

    public MessengerViewModel(Section section)
    {
        this.WhenActivated(disposables =>
        {
            this.BindCatalog(section).DisposeWith(disposables);
            this.BindInformer().DisposeWith(disposables);
            this.BindExplorer().DisposeWith(disposables);
            this.BindHome().DisposeWith(disposables);
            this.BindEditor().DisposeWith(disposables);
            this.BindNotifications().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}