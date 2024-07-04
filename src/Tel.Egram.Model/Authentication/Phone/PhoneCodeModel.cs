using Avalonia.Media.Imaging;
using PropertyChanged;

namespace Tel.Egram.Model.Authentication.Phone;

[AddINotifyPropertyChangedInterface]
public class PhoneCodeModel
{
    public string Code { get; init; } = string.Empty;
        
    public string CountryCode { get; init; } = string.Empty;
        
    public Bitmap? Flag { get; init; }
        
    public string Mask { get; init; } = string.Empty;
}