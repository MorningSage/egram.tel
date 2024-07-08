using Avalonia.Media.Imaging;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public class Preview
{
    public Bitmap? Bitmap { get; set; }
        
    public PreviewQuality Quality { get; set; }
}