using System.Reactive.Disposables;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Special;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Persistence;

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
        
    public DocumentMessageViewModel(IFileLoader fileLoader, IFileExplorer fileExplorer, IAvatarLoader avatarLoader, IPreviewLoader previewLoader)
    {       
        this.WhenActivated(disposables =>
        {
            MessageModel?.BindAvatarLoading(avatarLoader).DisposeWith(disposables);
            MessageModel?.Reply?.BindPreviewLoading(previewLoader).DisposeWith(disposables);

            this.BindDownloadCommand(fileLoader).DisposeWith(disposables);
            this.BindShowFileCommand(fileExplorer).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}