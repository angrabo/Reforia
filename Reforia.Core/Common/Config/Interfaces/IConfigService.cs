
using Reforia.Core.Common.Config.Contracts;

namespace Reforia.Core.Common.Config.Interfaces;

public interface IConfigService
{
    Task<string> Get(EConfigOptions key, string defaultValue = "");
    Task Set(EConfigOptions key, string value);
    Task<bool> Remove(EConfigOptions key);
    IDictionary<string, string> GetAll();
}