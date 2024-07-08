using System.Reactive.Disposables;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Explorer.Messages;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Basic;

public class TextMessageViewModel : IActivatableViewModel
{
    public string Text { get; set; }
    public MessageModel? MessageModel { get; set; }
    
    public TextMessageViewModel()
    {
        this.WhenActivated(disposables =>
        {
            MessageModel?.BindAvatarLoading().DisposeWith(disposables);

            MessageModel?.Reply?.BindPreviewLoading().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}