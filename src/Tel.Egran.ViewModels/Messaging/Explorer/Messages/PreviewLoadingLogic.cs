using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages;

public static class PreviewLoadingLogic
{
    private static readonly IPreviewLoader PreviewLoader = Registry.Services.GetRequiredService<IPreviewLoader>();
    
    public static IDisposable BindPreviewLoading(this ReplyModel model)
    {
        if (model.Preview != null) return Disposable.Empty;
        
        model.Preview = GetPreview(model);

        if (model.Preview?.Bitmap == null)
        {
            return LoadPreview(model)
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

    public static IDisposable BindPreviewLoading(this PhotoMessageViewModel model)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }
        
    public static IDisposable BindPreviewLoading(this VideoMessageViewModel model)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }
        
    public static IDisposable BindPreviewLoading(this StickerMessageViewModel model)
    {
        if (model.VisualMessage is not { Preview: null }) return Disposable.Empty;
        
        model.VisualMessage.Preview = GetPreview(model);

        if (model.VisualMessage.Preview?.Bitmap == null)
        {
            return LoadPreview(model)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(preview =>
                {
                    model.VisualMessage.Preview = preview;
                });
        }

        return Disposable.Empty;
    }

    private static Preview? GetPreview(ReplyModel model)
    {
        if (model.PhotoData != null)
        {
            return PreviewLoader.GetPreview(model.PhotoData, PreviewQuality.Low);
        }

        if (model.VideoData?.Thumbnail != null)
        {
            return PreviewLoader.GetPreview(model.VideoData.Thumbnail);
        }

        if (model.StickerData?.Thumbnail != null)
        {
            return PreviewLoader.GetPreview(model.StickerData.Thumbnail);
        }

        return null;
    }

    private static IObservable<Preview> LoadPreview(ReplyModel model)
    {
        if (model.PhotoData != null)
        {
            return PreviewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low);
        }
            
        if (model.VideoData?.Thumbnail != null)
        {
            return PreviewLoader.LoadPreview(model.VideoData.Thumbnail);
        }
            
        if (model.StickerData?.Thumbnail != null)
        {
            return PreviewLoader.LoadPreview(model.StickerData.Thumbnail);
        }
            
        return Observable.Empty<Preview>();
    }
        
    private static Preview? GetPreview(PhotoMessageViewModel model) => model.PhotoData != null
        ? PreviewLoader.GetPreview(model.PhotoData, PreviewQuality.High)
        : null;

    private static IObservable<Preview> LoadPreview(PhotoMessageViewModel model) => model.PhotoData != null
        ? PreviewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low).Concat(PreviewLoader.LoadPreview(model.PhotoData, PreviewQuality.High))
        : Observable.Empty<Preview>();

    private static Preview? GetPreview(VideoMessageViewModel model) => model.VideoData?.Thumbnail != null
        ? PreviewLoader.GetPreview(model.VideoData.Thumbnail)
        : null;

    private static IObservable<Preview> LoadPreview(VideoMessageViewModel model) => model.VideoData?.Thumbnail != null
        ? PreviewLoader.LoadPreview(model.VideoData.Thumbnail)
        : Observable.Empty<Preview>();

    private static Preview? GetPreview(StickerMessageViewModel model) => model.StickerData?.Thumbnail != null
        ? PreviewLoader.GetPreview(model.StickerData.Thumbnail)
        : null;

    private static IObservable<Preview> LoadPreview(StickerMessageViewModel model)
    {
        if (model.StickerData == null) return Observable.Empty<Preview>();
        
        return model.StickerData.Thumbnail != null
            ? PreviewLoader.LoadPreview(model.StickerData.Thumbnail).Concat(PreviewLoader.LoadPreview(model.StickerData))
            : PreviewLoader.LoadPreview(model.StickerData);
    }
        
}