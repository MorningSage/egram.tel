using System.Reactive.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData.Binding;
using Tel.Egram.Model.Authentication.Phone;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Authentication.Phone;

public static class AuthenticationPhoneHelper
{
    public static IDisposable LoadPhoneCodes(this AuthenticationViewModel viewModel, IResourceManager resourceManager) => Task.Run(() =>
    {
        // ToDo: Add default items if something below fails
        var codes = resourceManager.GetPhoneCodes();

        viewModel.PhoneCodes = new ObservableCollectionExtended<PhoneCodeModel>(codes.Select(c => new PhoneCodeModel
        {
            Code         = $"+{c.Code}",
            CountryCode  = c.CountryCode,
            Flag         = GetFlag(c.CountryCode),
            Mask         = c.Mask.ToLowerInvariant()
        }).OrderBy(m => m.CountryCode));

        viewModel.PhoneCode = viewModel.PhoneCodes.FirstOrDefault(c => c.CountryCode == "RU", new PhoneCodeModel());
    })
    .ToObservable()
    .Accept();
    
    private static Bitmap GetFlag(string countryCode)
    {
        // ToDo: Consider moving flags to an Assets project?
        var uri = new Uri($"avares://Tel.Egram.Application/Images/Flags/{countryCode}.png");
        
        if (!AssetLoader.Exists(uri))
        {
            uri = new Uri("avares://Tel.Egram.Application/Images/Flags/_unknown.png");
        }

        using var stream = AssetLoader.Open(uri);
        return new Bitmap(stream);
    }
}