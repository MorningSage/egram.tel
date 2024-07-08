using System.Reactive;
using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;

namespace Tel.Egran.ViewModels.Messaging.Editor;

[AddINotifyPropertyChangedInterface]
public class EditorViewModel : IActivatableViewModel
{
    public bool IsVisible { get; set; } = true;
    public string? Text { get; set; }
        
    public ReactiveCommand<Unit, Unit>? SendCommand { get; set; }
        
    public EditorViewModel(Chat chat, IMessageSender messageSender)
    {
        this.WhenActivated(disposables => { this.BindSender(chat, messageSender).DisposeWith(disposables); });
    }

    private EditorViewModel() { }
        
    public static EditorViewModel Hidden() => new() { IsVisible = false };

    public ViewModelActivator Activator { get; } = new();
}