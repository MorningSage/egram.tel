using System.Diagnostics;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Tel.Egram.Model.Authentication.Phone;

namespace Tel.Egram.Services.Persistence;

public class ResourceManager : IResourceManager
{
    public IList<PhoneCodeModel> GetPhoneCodes()
    {
        var codes = new List<PhoneCodeModel>();
            
        var uri = new Uri("avares://Tel.Egram.Application/Resources/PhoneCodes.txt");
        if (!AssetLoader.Exists(uri))
        {
            // ToDo: Ensure that there's no issue during runtime *IF* the phone code list fails to load.
            Debugger.Break();
            return codes;
        }

        using var stream = new StreamReader(AssetLoader.Open(uri));
            
        while (!stream.EndOfStream)
        {
            var line = stream.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
                
            var parts = line.Split(';');

            var code        = parts.Length > 0 ? parts[0] : string.Empty;
            var countryCode = parts.Length > 1 ? parts[1] : string.Empty;
            var countryName = parts.Length > 2 ? parts[2] : string.Empty;
            var mask        = parts.Length > 3 ? parts[3] : string.Empty;
            var flag        = GetFlag(countryCode);

            codes.Add(new PhoneCodeModel
            {
                Code        = $"+{code}",
                CountryCode = countryCode,
                CountryName = countryName,
                Mask        = mask.ToLowerInvariant(),
                Flag        = flag
            });
        }

        return codes;
    }
    
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