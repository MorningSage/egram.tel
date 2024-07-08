using ReactiveUI;
using Tel.Egram.Services.Persistence;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Special;

/// <summary>
/// Logic for showing document in explorer or finder
/// </summary>
public static class DocumentExplorer
{
    public static IDisposable BindShowFileCommand(this DocumentMessageViewModel viewModel, IFileExplorer fileExplorer)
    {
        viewModel.ShowCommand = ReactiveCommand.Create((DocumentMessageViewModel m) => Explore(m, fileExplorer), null, RxApp.MainThreadScheduler);
        return viewModel.ShowCommand.Subscribe();
    }
        
    private static bool Explore(DocumentMessageViewModel viewModel, IFileExplorer fileExplorer)
    {
        if (viewModel.Document.Document_?.Local is not { } asdf || new FileInfo(asdf.Path) is not { Exists: true } fileInfo) return false;
        fileExplorer.OpenDirectory(fileInfo);
        return true;
    }
}