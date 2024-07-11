using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Messages.Visual;

public partial class PhotoMessageViewModel : AbstractMessageViewModel<AbstractVisualMessageModel>
{
    private readonly IPreviewLoader _previewLoader;
    
    [ObservableProperty] private string _text;
    [ObservableProperty] private TdApi.Photo? _photoData;
        
    public PhotoMessageViewModel(IPreviewLoader previewLoader, IAvatarLoader avatarLoader) : base(avatarLoader, previewLoader)
    {
        _previewLoader = previewLoader;
        
        BindPreviewLoading();
    }

    private void BindPreviewLoading()
    {
        if (MessageModel is not { Preview: null }) return;
        
        MessageModel.Preview = PhotoData is not null ? _previewLoader.GetPreview(PhotoData, PreviewQuality.High) : null;

        if (MessageModel.Preview?.Bitmap is not null || PhotoData is null) return;
        
        _previewLoader
            .LoadPreview(PhotoData, PreviewQuality.Low)
            .Concat(_previewLoader.LoadPreview(PhotoData, PreviewQuality.High))
            .SafeSubscribe(preview => MessageModel.Preview = preview);
    }
}