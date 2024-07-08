using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Informer;

[AddINotifyPropertyChangedInterface]
public class InformerViewModel : IActivatableViewModel
{
    public bool IsVisible { get; set; } = true;
        
    public string Title { get; set; }
        
    public string Label { get; set; }
        
    public Avatar Avatar { get; set; }
        
    public InformerViewModel(Chat chat, IAvatarLoader avatarLoader)
    {
        this.WhenActivated(disposables => this.BindInformer(chat, avatarLoader).DisposeWith(disposables));
    }

    public InformerViewModel(Aggregate aggregate)
    {
        this.WhenActivated(disposables => this.BindInformer(aggregate).DisposeWith(disposables));
    }

    private InformerViewModel() { }
    
    public static InformerViewModel Hidden()
    {
        return new InformerViewModel
        {
            IsVisible = false
        };
    }
        
    public ViewModelActivator Activator { get; } = new();
}