using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Messages;

public abstract partial class AbstractMessageViewModel<T> : AbstractViewModelBase where T : MessageModel
{
    private readonly IAvatarLoader _avatarLoader;
    private readonly IPreviewLoader _previewLoader;
    
    [ObservableProperty] private T? _messageModel;
    
    protected AbstractMessageViewModel(IAvatarLoader avatarLoader, IPreviewLoader previewLoader)
    {
        _avatarLoader = avatarLoader;
        _previewLoader = previewLoader;
        
        BindAvatarLoading();
        BindPreviewLoading();
    }
    
    private void BindAvatarLoading()
    {
        if (MessageModel is null || MessageModel.Avatar != null) return;
        
        MessageModel.Avatar = GetAvatar();

        if (MessageModel.Avatar?.IsFallback is null or true) LoadAvatar().SafeSubscribe(avatar =>
        {
            MessageModel.Avatar = avatar;
        });
    }
    
    private void BindPreviewLoading()
    {
        if (MessageModel?.Reply is null || MessageModel.Reply.Preview is not null) return;
        
        MessageModel.Reply.Preview = GetPreview();

        if (MessageModel.Reply.Preview?.Bitmap == null) LoadPreview().SafeSubscribe(preview =>
        {
            MessageModel.Reply.Preview    = preview;
            MessageModel.Reply.HasPreview = true;
        });

        MessageModel.Reply.HasPreview = true;
    }
    
    private Preview? GetPreview()
    {
        if (MessageModel?.Reply is null || MessageModel.Reply.Preview is not null) return null;
        
        if (MessageModel.Reply.PhotoData != null)
        {
            return _previewLoader.GetPreview(MessageModel.Reply.PhotoData, PreviewQuality.Low);
        }

        if (MessageModel.Reply.VideoData?.Thumbnail != null)
        {
            return _previewLoader.GetPreview(MessageModel.Reply.VideoData.Thumbnail);
        }

        if (MessageModel.Reply.StickerData?.Thumbnail != null)
        {
            return _previewLoader.GetPreview(MessageModel.Reply.StickerData.Thumbnail);
        }

        return null;
    }

    private IObservable<Preview> LoadPreview()
    {
        if (MessageModel?.Reply?.PhotoData != null)
        {
            return _previewLoader.LoadPreview(MessageModel.Reply.PhotoData, PreviewQuality.Low);
        }
            
        if (MessageModel?.Reply?.VideoData?.Thumbnail != null)
        {
            return _previewLoader.LoadPreview(MessageModel.Reply.VideoData.Thumbnail);
        }
            
        if (MessageModel?.Reply?.StickerData?.Thumbnail != null)
        {
            return _previewLoader.LoadPreview(MessageModel.Reply.StickerData.Thumbnail);
        }
            
        return Observable.Empty<Preview>();
    }
    
    private Avatar? GetAvatar()
    {
        if (MessageModel?.Message?.UserData != null)
        {
            return _avatarLoader.GetAvatar(MessageModel?.Message.UserData, AvatarSize.Regular);
        }

        if (MessageModel?.Message?.ChatData != null)
        {
            return _avatarLoader.GetAvatar(MessageModel?.Message.ChatData, AvatarSize.Regular);
        }
            
        return null;
    }
        
    private IObservable<Avatar> LoadAvatar()
    {
        if (MessageModel?.Message?.UserData != null)
        {
            return _avatarLoader.LoadAvatar(MessageModel.Message.UserData, AvatarSize.Regular);
        }

        if (MessageModel?.Message?.ChatData != null)
        {
            return _avatarLoader.LoadAvatar(MessageModel.Message.ChatData, AvatarSize.Regular);
        }
            
        return Observable.Empty<Avatar>();
    }
}