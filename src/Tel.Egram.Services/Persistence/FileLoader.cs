using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Persistence;

public class FileLoader(IAgent agent) : IFileLoader
{
    public IObservable<TdApi.File> LoadFile(TdApi.File file, LoadPriority priority)
    {
        if (!IsDownloadingNeeded(file)) return Observable.Return(file);
        
        return agent.Execute(new TdApi.DownloadFile
            {
                FileId = file.Id,
                Priority = (int) priority
            })
            .SelectSeq(downloading => agent.Updates
                .OfType<TdApi.Update.UpdateFile>()
                .Select(u => u.File)
                .Where(f => f.Id == downloading.Id)
                .TakeWhile(IsDownloadingNeeded))
            .Concat(Observable.Defer(() => agent.Execute(new TdApi.GetFile { FileId = file.Id })));

    }

    private static bool IsDownloadingNeeded(TdApi.File file)
    {
        return file.Local is not { IsDownloadingCompleted: true }
               || file.Local.Path == null
               || !File.Exists(file.Local.Path);
    }
}