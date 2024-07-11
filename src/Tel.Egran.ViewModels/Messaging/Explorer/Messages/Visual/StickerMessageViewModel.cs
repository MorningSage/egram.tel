using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

public partial class StickerMessageViewModel : AbstractMessageViewModel<AbstractVisualMessageModel>
{
    private readonly IPreviewLoader _previewLoader;
    
    [ObservableProperty] private TdApi.Sticker? _stickerData;
        
    public StickerMessageViewModel(IPreviewLoader previewLoader, IAvatarLoader avatarLoader) : base(avatarLoader, previewLoader)
    {
        _previewLoader = previewLoader;
        
        BindPreviewLoading();
    }

    private void BindPreviewLoading()
    {
        if (MessageModel is not { Preview: null }) return;
        
        MessageModel.Preview = StickerData?.Thumbnail != null ? _previewLoader.GetPreview(StickerData.Thumbnail) : null;

        if (MessageModel.Preview?.Bitmap is not null || StickerData is null) return;

        var preview = StickerData.Thumbnail != null
            ? _previewLoader.LoadPreview(StickerData.Thumbnail).Concat(_previewLoader.LoadPreview(StickerData))
            : _previewLoader.LoadPreview(StickerData);

        preview.SafeSubscribe(a => MessageModel.Preview = a);
    }
}