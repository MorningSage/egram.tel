using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages;

public static class PreviewLoadingLogic
{
    public static IDisposable BindPreviewLoading(this ReplyModel model, IPreviewLoader previewLoader)
    {
        if (model.Preview != null) return Disposable.Empty;
        
        model.Preview = GetPreview(model, previewLoader);

        if (model.Preview?.Bitmap == null)
        {
            return LoadPreview(model, previewLoader)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.Preview    = preview;
                    model.HasPreview = true;
                });
        }

        model.HasPreview = true;

        return Disposable.Empty;
    }

    public static IDisposable BindPreviewLoading(this PhotoMessageViewModel model, IPreviewLoader previewLoader)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model, previewLoader);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model, previewLoader)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }
        
    public static IDisposable BindPreviewLoading(this VideoMessageViewModel model, IPreviewLoader previewLoader)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model, previewLoader);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model, previewLoader)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }
        
    public static IDisposable BindPreviewLoading(this StickerMessageViewModel model, IPreviewLoader previewLoader)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model, previewLoader);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model, previewLoader)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }

    private static Preview? GetPreview(ReplyModel model, IPreviewLoader previewLoader)
    {
        if (model.PhotoData != null)
        {
            return previewLoader.GetPreview(model.PhotoData, PreviewQuality.Low);
        }

        if (model.VideoData?.Thumbnail != null)
        {
            return previewLoader.GetPreview(model.VideoData.Thumbnail);
        }

        if (model.StickerData?.Thumbnail != null)
        {
            return previewLoader.GetPreview(model.StickerData.Thumbnail);
        }

        return null;
    }

    private static IObservable<Preview> LoadPreview(ReplyModel model, IPreviewLoader previewLoader)
    {
        if (model.PhotoData != null)
        {
            return previewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low);
        }
            
        if (model.VideoData?.Thumbnail != null)
        {
            return previewLoader.LoadPreview(model.VideoData.Thumbnail);
        }
            
        if (model.StickerData?.Thumbnail != null)
        {
            return previewLoader.LoadPreview(model.StickerData.Thumbnail);
        }
            
        return Observable.Empty<Preview>();
    }
        
    private static Preview? GetPreview(PhotoMessageViewModel model, IPreviewLoader previewLoader) => model.PhotoData != null
        ? previewLoader.GetPreview(model.PhotoData, PreviewQuality.High)
        : null;

    private static IObservable<Preview> LoadPreview(PhotoMessageViewModel model, IPreviewLoader previewLoader) => model.PhotoData != null
        ? previewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low).Concat(previewLoader.LoadPreview(model.PhotoData, PreviewQuality.High))
        : Observable.Empty<Preview>();

    private static Preview? GetPreview(VideoMessageViewModel model, IPreviewLoader previewLoader) => model.VideoData?.Thumbnail != null
        ? previewLoader.GetPreview(model.VideoData.Thumbnail)
        : null;

    private static IObservable<Preview> LoadPreview(VideoMessageViewModel model, IPreviewLoader previewLoader) => model.VideoData?.Thumbnail != null
        ? previewLoader.LoadPreview(model.VideoData.Thumbnail)
        : Observable.Empty<Preview>();

    private static Preview? GetPreview(StickerMessageViewModel model, IPreviewLoader previewLoader) => model.StickerData?.Thumbnail != null
        ? previewLoader.GetPreview(model.StickerData.Thumbnail)
        : null;

    private static IObservable<Preview> LoadPreview(StickerMessageViewModel model, IPreviewLoader previewLoader)
    {
        if (model.StickerData == null) return Observable.Empty<Preview>();
        
        return model.StickerData.Thumbnail != null
            ? previewLoader.LoadPreview(model.StickerData.Thumbnail).Concat(previewLoader.LoadPreview(model.StickerData))
            : previewLoader.LoadPreview(model.StickerData);
    }
        
}