using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

public class PhotoMessageViewModel : IActivatableViewModel
{
    public string Text { get; set; }
    public AbstractVisualMessageModel? VisualMessage { get; set; }
        
    public TdApi.Photo? PhotoData { get; set; }
        
    public PhotoMessageViewModel()
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