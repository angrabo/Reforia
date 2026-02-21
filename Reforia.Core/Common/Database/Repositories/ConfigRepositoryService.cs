using Reforia.Core.Common.Config;
using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Database.Repositories;

public class ConfigRepositoryService : GenericRepositoryService<ConfigItem>
{
    private readonly IConfigRepository _configRepository;

    public ConfigRepositoryService(IConfigRepository configRepository) : base(configRepository)
    {
        _configRepository = configRepository;
    }

    public Task<ConfigItem?> GetByKeyAsync(string key, CancellationToken ct = default) =>
        _configRepository.GetByKeyAsync(key, ct);
}