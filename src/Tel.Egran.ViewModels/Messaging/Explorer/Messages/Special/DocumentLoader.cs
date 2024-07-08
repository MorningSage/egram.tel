using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Services.Persistence;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Special;

/// <summary>
/// File loading logic for documents
/// </summary>
public static class DocumentLoader
{
    public static IDisposable BindDownloadCommand(this DocumentMessageViewModel viewModel, IFileLoader fileLoader)
    {
        var file = viewModel.Document.Document_;
        
        viewModel.DownloadCommand = ReactiveCommand.CreateFromObservable((DocumentMessageViewModel m) => Download(m, fileLoader), null, RxApp.MainThreadScheduler);
        viewModel.IsDownloaded = (file.Local?.IsDownloadingCompleted ?? false) && File.Exists(file.Local?.Path);
        
        return viewModel.DownloadCommand.Subscribe(isDownloaded => { viewModel.IsDownloaded = isDownloaded; });
    }

    private static IObservable<bool> Download(DocumentMessageViewModel viewModel, IFileLoader fileLoader) => fileLoader
        .LoadFile(viewModel.Document.Document_, LoadPriority.Mid)
        .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
        .SubscribeOn(RxApp.TaskpoolScheduler)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Select(f => f.Local.IsDownloadingCompleted);
}