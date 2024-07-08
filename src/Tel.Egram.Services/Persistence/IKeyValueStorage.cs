using System.Diagnostics.CodeAnalysis;

namespace Tel.Egram.Services.Persistence;

public interface IKeyValueStorage
{
    void Set<T>(string key, T value) where T : struct;
        
    T Get<T>(string key) where T : struct;

    IList<KeyValuePair<string, T>> GetAll<T>() where T : struct;
        
    bool TryGet<T>(string key, [NotNullWhen(true)] out T? value) where T : struct;

    void Delete(string key);
}