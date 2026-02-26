using Reforia.Infrastructure.LiteDb;
using Reforia.Infrastructure.LiteDb.Interfaces;

namespace Reforia.Shared.Config;

public class ConfigService : IConfigService
{
    private readonly IRepository<ConfigItem>    _repository;
    private readonly Dictionary<string, string> _cache;

    public ConfigService(IRepository<ConfigItem> repository)
    {
        _repository = repository;
        _cache = new Dictionary<string, string>();

        foreach (var item in _repository.GetAll())
        {
            _cache[item.Key] = item.Value;
        }
    }

    public string? Get(string key, string? defaultValue = null)
    {
        if (_cache.TryGetValue(key, out var value))
            return value;

        var item = _repository.Find(x => x.Key == key).FirstOrDefault();
        if (item != null)
        {
            _cache[key] = item.Value;
            return item.Value;
        }

        return defaultValue;
    }

    public void Set(string key, string value)
    {
        _cache[key] = value;

        var existing = _repository.Find(x => x.Key == key).FirstOrDefault();
        if (existing != null)
        {
            existing.Value = value;
            _repository.Update(existing);
        }
        else
        {
            _repository.Insert(new ConfigItem { Key = key, Value = value });
        }
    }

    public bool Remove(string key)
    {
        _cache.Remove(key);

        var existing = _repository.Find(x => x.Key == key).FirstOrDefault();
        if (existing != null)
        {
            return _repository.Delete(existing.Id);
        }
        return false;
    }

    public IDictionary<string, string> GetAll()
    {
        return new Dictionary<string, string>(_cache);
    }
}