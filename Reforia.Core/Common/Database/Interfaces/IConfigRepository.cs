using Reforia.Core.Common.Config;

namespace Reforia.Core.Common.Database.Interfaces;

public interface IConfigRepository<T> : IRepository<ConfigItem>
{
    public Task<T?> GetByKeyAsync(string key, CancellationToken ct = default);

}