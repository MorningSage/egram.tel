using Avalonia.Media.Imaging;
using TdLib;
using Tel.Egram.Services.Persistence;

namespace Tel.Egram.Services.Graphics;

public interface IBitmapLoader
{
    IObservable<Bitmap> LoadFile(TdApi.File file, LoadPriority priority);
}