using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

public class VideoMessageViewModel : IActivatableViewModel
{
    public AbstractVisualMessageModel? VisualMessage { get; set; }
    public string Text { get; set; }
        
    public TdApi.Video? VideoData { get; set; }
        
    public VideoMessageViewModel(IPreviewLoader previewLoader, IAvatarLoader avatarLoader)
    {
        this.WhenActivated(disposables =>
        {
            VisualMessage?.Reply?.BindPreviewLoading(previewLoader).DisposeWith(disposables);
            
            VisualMessage?.BindAvatarLoading(avatarLoader).DisposeWith(disposables);
            this.BindPreviewLoading(previewLoader).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}