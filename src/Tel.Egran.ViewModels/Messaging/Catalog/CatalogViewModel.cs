using System.Reactive.Disposables;
using System.Reactive.Subjects;
using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egran.ViewModels.Messaging.Catalog;

[AddINotifyPropertyChangedInterface]
public class CatalogViewModel : IActivatableViewModel
{
    public bool IsVisible { get; set; } = true;
        
    public EntryViewModel? SelectedEntry { get; set; }
        
    public ObservableCollectionExtended<EntryViewModel> Entries { get; set; } = [];
        
    public string SearchText { get; set; }
        
    public Subject<IComparer<EntryViewModel>> SortingController { get; set; } = new();
        
    public Subject<Func<EntryViewModel, bool>> FilterController { get; set; } = new();

    public CatalogViewModel(Section section)
    {
        this.WhenActivated(disposables =>
        {
            this.BindCatalogFilter(section).DisposeWith(disposables);
            this.BindChats(section).DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();
}