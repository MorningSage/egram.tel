using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Tel.Egram.Services;
using Tel.Egram.Services.Persistence;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Special;

/// <summary>
/// Logic for showing document in explorer or finder
/// </summary>
public static class DocumentExplorer
{
    private static readonly IFileExplorer FileExplorer = Registry.Services.GetRequiredService<IFileExplorer>();
    
    public static IDisposable BindShowFileCommand(this DocumentMessageViewModel viewModel)
    {
        viewModel.ShowCommand = ReactiveCommand.Create((DocumentMessageViewModel m) => Explore(m), null, RxApp.MainThreadScheduler);
        return viewModel.ShowCommand.Subscribe();
    }
        
    private static bool Explore(DocumentMessageViewModel viewModel)
    {
        if (viewModel.Document.Document_?.Local is not { } asdf || new FileInfo(asdf.Path) is not { Exists: true } fileInfo) return false;
        FileExplorer.OpenDirectory(fileInfo);
        return true;
    }
}