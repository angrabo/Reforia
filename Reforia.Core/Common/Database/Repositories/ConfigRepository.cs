using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Config;
using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Database.Repositories;

public class ConfigRepository : ARepository<ConfigItem>, IConfigRepository
{
    public ConfigRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<ConfigItem?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Key == key, ct);
    }
}