using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

namespace Tel.Egram.Services.Graphics.Previews;

public interface IPreviewLoader
{
    Preview GetPreview(TdApi.Photo photo, PreviewQuality quality);

    IObservable<Preview> LoadPreview(TdApi.Photo photo, PreviewQuality quality);

    Preview GetPreview(TdApi.PhotoSize photoSize);
        
    IObservable<Preview> LoadPreview(TdApi.PhotoSize photoSize);

    Preview GetPreview(TdApi.Sticker sticker);

    IObservable<Preview> LoadPreview(TdApi.Sticker sticker);

    Preview GetPreview(TdApi.Thumbnail thumbnail);

    IObservable<Preview> LoadPreview(TdApi.Thumbnail thumbnail);
}