using TdLib;

namespace Tel.Egram.Services.Persistence;

public interface IFileLoader
{
    IObservable<TdApi.File> LoadFile(TdApi.File file, LoadPriority priority);
}