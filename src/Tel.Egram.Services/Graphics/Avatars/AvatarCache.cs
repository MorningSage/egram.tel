using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace Tel.Egram.Services.Graphics.Avatars;

public class AvatarCache(IMemoryCache cache) : IAvatarCache
{
    public bool TryGetValue(object key, [NotNullWhen(true)] out object? value) => cache.TryGetValue(key, out value);

    public ICacheEntry CreateEntry(object key) => cache.CreateEntry(key);

    public void Remove(object key) => cache.Remove(key);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        cache.Dispose();
    }
}