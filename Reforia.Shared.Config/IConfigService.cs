namespace Reforia.Shared.Config;

public interface IConfigService
{
    string? Get(string key, string defaultValue = "");
    void Set(string key, string value);
    bool Remove(string key);
    IDictionary<string, string> GetAll();
}