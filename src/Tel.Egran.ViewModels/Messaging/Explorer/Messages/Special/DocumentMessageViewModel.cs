using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Special;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Special;

public class DocumentMessageViewModel : IActivatableViewModel
{
    public DocumentMessageModel? MessageModel { get; set; }
    
    public TdApi.Document Document { get; set; }
        
    public bool IsDownloaded { get; set; }
        
    public string Name { get; set; }
        
    public string Text { get; set; }
        
    public string Size { get; set; }
        
    public ReactiveCommand<DocumentMessageViewModel, bool> DownloadCommand { get; set; }
        
    public ReactiveCommand<DocumentMessageViewModel, bool> ShowCommand { get; set; }
        
    public DocumentMessageViewModel()
    {       
        this.WhenActivated(disposables =>
        {
            MessageModel?.BindAvatarLoading().DisposeWith(disposables);
            MessageModel?.Reply?.BindPreviewLoading().DisposeWith(disposables);

            this.BindDownloadCommand().DisposeWith(disposables);
            this.BindShowFileCommand().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}