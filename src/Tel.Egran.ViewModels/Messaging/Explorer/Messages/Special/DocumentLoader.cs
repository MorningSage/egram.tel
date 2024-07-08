using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Services;
using Tel.Egram.Services.Persistence;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Special;

/// <summary>
/// File loading logic for documents
/// </summary>
public static class DocumentLoader
{
    private static readonly IFileLoader FileLoader = Registry.Services.GetRequiredService<IFileLoader>();

    public static IDisposable BindDownloadCommand(this DocumentMessageViewModel viewModel)
    {
        var file = viewModel.Document.Document_;
        
        viewModel.DownloadCommand = ReactiveCommand.CreateFromObservable((DocumentMessageViewModel m) => Download(m), null, RxApp.MainThreadScheduler);
        viewModel.IsDownloaded = (file.Local?.IsDownloadingCompleted ?? false) && File.Exists(file.Local?.Path);
        
        return viewModel.DownloadCommand.Subscribe(isDownloaded => { viewModel.IsDownloaded = isDownloaded; });
    }

    private static IObservable<bool> Download(DocumentMessageViewModel viewModel) => FileLoader
        .LoadFile(viewModel.Document.Document_, LoadPriority.Mid)
        .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
        .SubscribeOn(RxApp.TaskpoolScheduler)
        .ObserveOn(RxApp.MainThreadScheduler)
        .Select(f => f.Local.IsDownloadingCompleted);
}