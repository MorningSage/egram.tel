using System.Reactive.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData.Binding;
using Tel.Egram.Services.Persistance;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.Model.Authentication.Phone;

public class PhoneCodeLoader
{
    private readonly IResourceManager _resourceManager;

    public PhoneCodeLoader(
        IResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public IDisposable Bind(AuthenticationModel model)
    {
        return Task.Run(() =>
            {
                //var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var codes = _resourceManager.GetPhoneCodes();

                model.PhoneCodes = new ObservableCollectionExtended<PhoneCodeModel>(
                    codes
                        .Select(c => new PhoneCodeModel
                        {
                            Code = "+" + c.Code,
                            CountryCode = c.CountryCode,
                            Flag = GetFlag(c.CountryCode),
                            Mask = c.Mask?.ToLowerInvariant()
                        })
                        .OrderBy(m => m.CountryCode));

                model.PhoneCode = model.PhoneCodes.FirstOrDefault(c => c.CountryCode == "RU");
            })
            .ToObservable()
            .Accept();
    }

    private static Bitmap GetFlag(string countryCode)
    {
        var uri = new Uri($"avares://Tel.Egram.Application/Images/Flags/{countryCode}.png");
        if (!AssetLoader.Exists(uri))
        {
            uri = new Uri("avares://Tel.Egram.Application/Images/Flags/_unknown.png");
        }

        using var stream = AssetLoader.Open(uri);
        return new Bitmap(stream);
    }
}