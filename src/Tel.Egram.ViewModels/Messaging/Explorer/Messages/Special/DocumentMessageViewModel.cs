using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Special;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Persistence;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Messages.Special;

public partial class DocumentMessageViewModel : AbstractMessageViewModel<DocumentMessageModel>
{
    private readonly IFileLoader _fileLoader;
    private readonly IFileExplorer _fileExplorer;
    
    [ObservableProperty] private TdApi.Document _document;
    [ObservableProperty] private bool _isDownloaded;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _text;
    [ObservableProperty] private string _size;
    
    public DocumentMessageViewModel(IFileLoader fileLoader, IFileExplorer fileExplorer, IAvatarLoader avatarLoader, IPreviewLoader previewLoader) : base(avatarLoader, previewLoader)
    {
        _fileLoader = fileLoader;
        _fileExplorer = fileExplorer;
        
        var file = Document.Document_;
        IsDownloaded = (file.Local?.IsDownloadingCompleted ?? false) && File.Exists(file.Local?.Path);
    }

    [RelayCommand]
    private void Download() => _fileLoader
        .LoadFile(Document.Document_, LoadPriority.Mid)
        .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
        .Select(f => f.Local.IsDownloadingCompleted)
        .Subscribe(b => IsDownloaded = b);

    [RelayCommand]
    private void Show()
    {
        if (Document.Document_?.Local is not { } localFile || new FileInfo(localFile.Path) is not { Exists: true } fileInfo) return;
        _fileExplorer.OpenDirectory(fileInfo);
    }
}