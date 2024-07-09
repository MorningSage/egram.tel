using Avalonia.Media.Imaging;

namespace Tel.Egram.Model.Authentication.Phone;

public class PhoneCodeModel
{
    public required string Code { get; set; }
    public required string CountryCode { get; set; }
    public required Bitmap? Flag { get; set; }
    public required string Mask { get; set; }
}