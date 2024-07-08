using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reactive.Linq;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TdLib;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Utils.Platforms;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Color = Avalonia.Media.Color;
using DrawingImage = System.Drawing.Image;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingGraphics = System.Drawing.Graphics;
using DrawingRectangle = System.Drawing.Rectangle;

namespace Tel.Egram.Services.Graphics.Avatars;

public class AvatarLoader(IPlatform platform, IStorage storage, IFileLoader fileLoader, IAvatarCache avatarCache, IColorMapper colorMapper) : IAvatarLoader, IDisposable
{
    private readonly object _locker = new();

    public Avatar GetAvatar(AvatarKind kind, AvatarSize size) => new()
    {
        Bitmap = null,
        Color  = GetColor(kind),
        Label  = GetLabel(kind)
    };

    public Avatar GetAvatar(TdApi.User user, AvatarSize size) => new()
    {
        Bitmap = GetBitmap(user.ProfilePhoto?.Small, platform.PixelDensity * (int) size),
        Color  = GetColor(user),
        Label  = GetLabel(user)
    };

    public Avatar GetAvatar(TdApi.Chat chat, AvatarSize size) => new()
    {
        Bitmap = GetBitmap(chat.Photo?.Small, platform.PixelDensity * (int) size),
        Color  = GetColor(chat),
        Label  = GetLabel(chat)
    };

    public IObservable<Avatar> LoadAvatar(AvatarKind kind, AvatarSize size) => Observable.Return(GetAvatar(kind, size));

    public IObservable<Avatar> LoadAvatar(TdApi.User user, AvatarSize size) => LoadBitmap(user.ProfilePhoto?.Small, platform.PixelDensity * (int) size)
        .Select(bitmap => new Avatar
        {
            Bitmap = bitmap,
            Color  = GetColor(user),
            Label  = GetLabel(user)
        });

    public IObservable<Avatar> LoadAvatar(TdApi.Chat chat, AvatarSize size) => LoadBitmap(chat.Photo?.Small, platform.PixelDensity * (int) size)
        .Select(bitmap => new Avatar
        {
            Bitmap = bitmap,
            Color  = GetColor(chat),
            Label  = GetLabel(chat)
        });

    private static string GetLabel(AvatarKind kind) => kind switch
    {
        AvatarKind.Home => "@",
        _               => string.Empty
    };

    private static string? GetLabel(TdApi.Chat chat) => string.IsNullOrWhiteSpace(chat.Title) ? null : chat.Title[..1].ToUpper();

    private static string? GetLabel(TdApi.User user)
    {
        if (!string.IsNullOrWhiteSpace(user.FirstName))
        {
            return !string.IsNullOrWhiteSpace(user.LastName)
                ? new string([ user.FirstName[0], user.LastName[0] ])
                : new string(user.FirstName[0], 1);
        }
        
        return !string.IsNullOrWhiteSpace(user.LastName) ? new string(user.LastName[0], 1) : null;
    }

    private Color GetColor(AvatarKind kind) => Color.Parse("#" + colorMapper[(int) kind]);
    private Color GetColor(TdApi.User user) => Color.Parse("#" + colorMapper[user.Id]);
    private Color GetColor(TdApi.Chat chat) => Color.Parse("#" + colorMapper[chat.Id]);

    private Bitmap? GetBitmap(TdApi.File? file, int size)
    {
        if (file?.Local?.Path == null) return null;
        
        return avatarCache.TryGetValue(GetResizedPath(file.Local.Path, size), out var value) && value is Bitmap bitmap
            ? bitmap
            : null;
    }

    private IObservable<Bitmap?> LoadBitmap(TdApi.File? file, int size) => file != null
        ? fileLoader.LoadFile(file, LoadPriority.Max)
            .FirstAsync(f => f.Local is { IsDownloadingCompleted: true })
            .SelectMany(f => GetBitmapAsync(f.Local.Path, size))
        : Observable.Return<Bitmap?>(null);

    private Task<Bitmap?> GetBitmapAsync(string filePath, int size)
    {
        var resizedFilePath = GetResizedPath(filePath, size);
            
        // return cached version
        if (avatarCache.TryGetValue(resizedFilePath, out var item) && item is Bitmap itemBitmap) return Task.FromResult<Bitmap?>(itemBitmap);
  
        // return resized version from disk
        if (File.Exists(resizedFilePath))
        {
            lock (_locker)
            {
                if (File.Exists(resizedFilePath))
                {
                    var bitmap = new Bitmap(resizedFilePath);
                    avatarCache.Set(resizedFilePath, bitmap, new MemoryCacheEntryOptions { Size = 1 });
                    return Task.FromResult<Bitmap?>(bitmap);
                }
            }
        }
            
        // resize and return image
        if (File.Exists(filePath))
        {
            return Task.Run(() =>
            {
                lock (_locker)
                {
                    if (File.Exists(filePath) && !File.Exists(resizedFilePath))
                    {
                        if (platform is WindowsPlatform)
                        {
                            ResizeWithSystemDrawing(filePath, resizedFilePath, size);
                        }
                        else
                        {
                            ResizeWithImageSharp(filePath, resizedFilePath, size);
                        }
                    }

                    if (!File.Exists(resizedFilePath)) return null;
                    
                    var bitmap = new Bitmap(resizedFilePath);
                    avatarCache.Set(resizedFilePath, bitmap, new MemoryCacheEntryOptions { Size = 1 });
                    return bitmap;

                }
            });
        }

        return Task.FromResult<Bitmap?>(null);
    }

    private string GetResizedPath(string localPath, int size)
    {   
        var originalName      = Path.GetFileNameWithoutExtension(localPath);
        var originalExtension = Path.GetExtension(localPath);

        return Path.Combine(storage.AvatarCacheDirectory, $"{originalName}_{size}{originalExtension}");
    }

    private static void ResizeWithSystemDrawing(string filePath, string resizedFilePath, int size)
    {
#if WINDOWS
        var image = DrawingImage.FromFile(filePath);

        var destRect = new DrawingRectangle(0, 0, size, size);
        var destImage = new DrawingBitmap(size, size);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = DrawingGraphics.FromImage(destImage))
        {
            graphics.CompositingMode    = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode  = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode      = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode    = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        destImage.Save(resizedFilePath);
#endif
    }

    private static void ResizeWithImageSharp(string filePath, string resizedFilePath, int size)
    {
        using var image = SixLabors.ImageSharp.Image.Load(filePath);
        image.Mutate(ctx=>ctx.Resize(size, size));
        image.Save(resizedFilePath);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        avatarCache.Dispose();
    }
}