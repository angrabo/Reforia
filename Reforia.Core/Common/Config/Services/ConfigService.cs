using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Config.Services;

public class ConfigService : IConfigService
{
    private readonly IConfigRepository _repository;
    private readonly Dictionary<string, string>    _cache;

    public ConfigService(IConfigRepository repository)
    {
        _repository = repository;
        _cache = new Dictionary<string, string>();

        foreach (var item in _repository.GetAllAsync().Result)
        {
            _cache[item.Key] = item.Value;
        }
    }

    public async Task<string> Get(EConfigOptions enumKey, string defaultValue = "")
    {
        var key = enumKey.ToString();
        if (_cache.TryGetValue(key, out var value))
            return value;

        var item = await _repository.GetByKeyAsync(key);
        if (item != null)
        {
            _cache[key] = item.Value;
            return item.Value;
        }

        return defaultValue;
    }

    public async Task Set(EConfigOptions enumKey, string value)
    {
        var key = enumKey.ToString();
        
        _cache[key] = value;

        var existing = await _repository.GetByKeyAsync(key);
        if (existing != null)
        {
            existing.Value = value;
            await _repository.UpdateAsync(existing);
        }
        else
        {
            await _repository.AddAsync(new ConfigItem { Key = key, Value = value });
        }
    }

    public async Task<bool> Remove(EConfigOptions enumKey)
    {
        var key = enumKey.ToString();
        _cache.Remove(key);

        var existing = await _repository.GetByKeyAsync(key);
        if (existing == null) return false;
        await _repository.DeleteAsync(existing.Id);
        return true;
    }

    public IDictionary<string, string> GetAll()
    {
        return new Dictionary<string, string>(_cache);
    }
}