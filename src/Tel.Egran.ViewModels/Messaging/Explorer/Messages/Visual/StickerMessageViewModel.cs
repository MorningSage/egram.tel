using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

public class StickerMessageViewModel : IActivatableViewModel
{
    public AbstractVisualMessageModel? VisualMessage { get; set; }
    public TdApi.Sticker? StickerData { get; set; }
        
    public StickerMessageViewModel()
    {
        this.WhenActivated(disposables =>
        {
            VisualMessage?.Reply?.BindPreviewLoading().DisposeWith(disposables);
            
            VisualMessage?.BindAvatarLoading().DisposeWith(disposables);
            this.BindPreviewLoading().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}