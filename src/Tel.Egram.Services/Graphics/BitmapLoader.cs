using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using TdLib;
using Tel.Egram.Services.Persistence;

namespace Tel.Egram.Services.Graphics;

public class BitmapLoader(IFileLoader fileLoader) : IBitmapLoader, IDisposable
{
    public IObservable<Bitmap> LoadFile(TdApi.File file, LoadPriority priority) => fileLoader
        .LoadFile(file, priority)
        .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
        .Select(f => new Bitmap(f.Local.Path));

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}