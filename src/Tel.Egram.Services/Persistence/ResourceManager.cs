using System.Diagnostics;
using Avalonia.Platform;
using Tel.Egram.Services.Persistence.Resources;

namespace Tel.Egram.Services.Persistence;

public class ResourceManager : IResourceManager
{
    public IList<PhoneCode> GetPhoneCodes()
    {
        var codes = new List<PhoneCode>();
            
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
            
            var code = new PhoneCode
            {
                Code        = parts.Length > 0 ? parts[0] : string.Empty,
                CountryCode = parts.Length > 1 ? parts[1] : string.Empty,
                CountryName = parts.Length > 2 ? parts[2] : string.Empty,
                Mask        = parts.Length > 3 ? parts[3] : string.Empty,
            };

            codes.Add(code);
        }

        return codes;
    }
}