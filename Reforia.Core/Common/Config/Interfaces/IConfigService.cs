namespace Reforia.Core.Common.Config.Interfaces;

public interface IConfigService
{
    Task<string?> Get(string key);
    Task Set(string key, string value);
    Task<bool> Remove(string key);
    IDictionary<string, string> GetAll();
}