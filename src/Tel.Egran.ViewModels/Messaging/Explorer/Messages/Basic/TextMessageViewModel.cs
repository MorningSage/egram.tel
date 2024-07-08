using System.Reactive.Disposables;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Basic;

public class TextMessageViewModel : IActivatableViewModel
{
    public string Text { get; set; }
    public MessageModel? MessageModel { get; set; }
    
    public TextMessageViewModel(IAvatarLoader avatarLoader, IPreviewLoader previewLoader)
    {
        this.WhenActivated(disposables =>
        {
            MessageModel?.BindAvatarLoading(avatarLoader).DisposeWith(disposables);
            MessageModel?.Reply?.BindPreviewLoading(previewLoader).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}