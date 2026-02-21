using Reforia.Core.Common.Config;

namespace Reforia.Core.Common.Database.Interfaces;

public interface IConfigRepositoryService : IService<ConfigItem>
{
    Task<ConfigItem?> GetByKeyAsync(string key, CancellationToken ct = default);
}
