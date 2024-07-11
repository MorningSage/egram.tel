using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Messages.Visual;

public partial class VideoMessageViewModel : AbstractMessageViewModel<AbstractVisualMessageModel>
{
    private readonly IPreviewLoader _previewLoader;
    
    [ObservableProperty] private string _text;
    [ObservableProperty] private TdApi.Video? _videoData;
        
    public VideoMessageViewModel(IPreviewLoader previewLoader, IAvatarLoader avatarLoader) : base(avatarLoader, previewLoader)
    {
        _previewLoader = previewLoader;
        
        BindPreviewLoading();
    }
    
    private void BindPreviewLoading()
    {
        if (MessageModel is not { Preview: null }) return;
        
        MessageModel.Preview = VideoData?.Thumbnail != null ? _previewLoader.GetPreview(VideoData.Thumbnail) : null;

        if (MessageModel.Preview?.Bitmap is not null || VideoData?.Thumbnail is null) return;
        
        _previewLoader.LoadPreview(VideoData.Thumbnail).SafeSubscribe(preview => MessageModel.Preview = preview);
    }
    
}