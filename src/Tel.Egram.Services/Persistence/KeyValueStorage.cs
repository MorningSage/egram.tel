using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tel.Egram.Services.Persistence.Entities;

namespace Tel.Egram.Services.Persistence;

public class KeyValueStorage(DatabaseContext db) : IKeyValueStorage
{
    public void Set<T>(string key, T value) where T : struct
    {
        var entity = db.Values.FirstOrDefault(v => v.Key == key);
        var obj = Serialize(value);
            
        if (entity != null)
        {
            entity.Value = obj;
            db.Values.Update(entity);
        }
        else
        {
            db.Values.Add(new KeyValueEntity
            {
                Key = key,
                Value = obj
            });
        }
            
        db.SaveChanges();
    }

    public T Get<T>(string key) where T : struct
    {
        var entity = db.Values.AsNoTracking().FirstOrDefault(v => v.Key == key);

        if (entity == null)
        {
            throw new NullReferenceException($"Value for key '{key}' is not set");
        }
            
        return Deserialize<T>(entity.Value);
    }

    public IList<KeyValuePair<string, T>> GetAll<T>() where T : struct => db.Values.AsNoTracking()
        .Select(v => new KeyValuePair<string, T>(v.Key, Deserialize<T>(v.Value)))
        .ToList();

    public bool TryGet<T>(string key, [NotNullWhen(true)] out T? value) where T : struct
    {
        var entity = db.Values.AsNoTracking().FirstOrDefault(v => v.Key == key);

        if (entity == null)
        {
            value = default;
            return false;
        }

        value = Deserialize<T>(entity.Value);
        return true;
    }

    public void Delete(string key)
    {
        var entity = db.Values.FirstOrDefault(v => v.Key == key);

        if (entity == null) return;
        
        db.Values.Remove(entity);
        db.SaveChanges();
    }

    private static string Serialize<T>(T obj) where T : struct => JsonConvert.SerializeObject(obj);

    private static T Deserialize<T>(string? v) where T : struct => v != null
        ? JsonConvert.DeserializeObject<T>(v)
        : default;
}