using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Persistence;

namespace Tel.Egram.Services.Graphics.Previews;

public class PreviewLoader(IFileLoader fileLoader, IPreviewCache previewCache) : IPreviewLoader
{
    private readonly object _locker = new();

    private static readonly string[] LowQualitySizes  = [ "m", "b", "a", "s" ];
    private static readonly string[] HighQualitySizes = [ "x", "c" ];

    public Preview GetPreview(TdApi.Photo photo, PreviewQuality quality)
    {
        var types = GetTypesByQuality(quality);
        var file = photo.Sizes.Where(s => Array.IndexOf(types, s.Type) >= 0).MinBy(s => Array.IndexOf(types, s.Type))?.Photo;
            
        return new Preview
        {
            Bitmap  = GetBitmap(file),
            Quality = PreviewQuality.High
        };
    }

    public IObservable<Preview> LoadPreview(TdApi.Photo photo, PreviewQuality quality)
    {
        var types = GetTypesByQuality(quality);
        var file = photo.Sizes.Where(s => Array.IndexOf(types, s.Type) >= 0).MinBy(s => Array.IndexOf(types, s.Type))?.Photo;

        return LoadBitmap(file).Select(bitmap => new Preview
        {
            Bitmap  = bitmap,
            Quality = quality
        });
    }

    public Preview GetPreview(TdApi.PhotoSize? photoSize) => new()
    {
        Bitmap  = GetBitmap(photoSize?.Photo),
        Quality = PreviewQuality.High
    };

    public IObservable<Preview> LoadPreview(TdApi.PhotoSize? photoSize) => LoadBitmap(photoSize?.Photo).Select(bitmap => new Preview
    {
        Bitmap  = bitmap,
        Quality = PreviewQuality.High
    });

    public Preview GetPreview(TdApi.Sticker sticker) => new()
    {
        Bitmap  = GetBitmap(sticker.Sticker_),
        Quality = PreviewQuality.High
    };

    public IObservable<Preview> LoadPreview(TdApi.Sticker sticker) => LoadBitmap(sticker.Sticker_).Select(bitmap => new Preview
    {
        Bitmap  = bitmap,
        Quality = PreviewQuality.High
    });

    private IObservable<Bitmap?> LoadBitmap(TdApi.File? file) => file != null
        ? fileLoader.LoadFile(file, LoadPriority.Mid)
            .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
            .Select(f => GetBitmap(f.Local.Path))
        : Observable.Return<Bitmap?>(null);

    private Bitmap? GetBitmap(TdApi.File? file)
    {
        return file?.Local?.Path != null && previewCache.TryGetValue(file.Local.Path, out var value) && value is Bitmap bitmap
            ? bitmap
            : null;
    }

    private Bitmap? GetBitmap(string filePath)
    {
        lock (_locker)
        {
            var bitmap = default(Bitmap?);
            
            if (previewCache.TryGetValue(filePath, out var item) && item is Bitmap itemBitmap)
            {
                bitmap = itemBitmap;
            }
            else if (File.Exists(filePath))
            {
                bitmap = new Bitmap(filePath);
                previewCache.Set(filePath, bitmap, new MemoryCacheEntryOptions { Size = 1 });
            }
            
            return bitmap;
        }
    }

    private static string[] GetTypesByQuality(PreviewQuality quality)
    {
        // s m x y w
        // a b c d
        return quality switch
        {
            PreviewQuality.Low  => LowQualitySizes,
            PreviewQuality.High => HighQualitySizes,
            _                   => []
        };
    }

    public Preview GetPreview(TdApi.Thumbnail thumbnail) => new()
    {
        Bitmap  = GetBitmap(thumbnail.File),
        Quality = PreviewQuality.High
    };

    public IObservable<Preview> LoadPreview(TdApi.Thumbnail thumbnail) => LoadBitmap(thumbnail.File).Select(bitmap => new Preview
    {
        Bitmap  = bitmap,
        Quality = PreviewQuality.High
    });
}