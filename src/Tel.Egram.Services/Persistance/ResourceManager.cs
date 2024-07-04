using System.Diagnostics;
using Avalonia.Platform;
using Tel.Egram.Services.Persistance.Resources;

namespace Tel.Egram.Services.Persistance;

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
                
            var code = new PhoneCode();
            var parts = line.Split(';');

            if (parts.Length > 0) code.Code = parts[0];
            if (parts.Length > 1) code.CountryCode = parts[1];
            if (parts.Length > 2) code.CountryName = parts[2];
            if (parts.Length > 3) code.Mask = parts[3];

            codes.Add(code);
        }

        return codes;
    }
}