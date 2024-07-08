using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

public class StickerMessageViewModel : IActivatableViewModel
{
    public AbstractVisualMessageModel? VisualMessage { get; set; }
    public TdApi.Sticker? StickerData { get; set; }
        
    public StickerMessageViewModel(IPreviewLoader previewLoader, IAvatarLoader avatarLoader)
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