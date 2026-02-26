using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Config;
using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Database.Repositories;

public class ConfigRepository : IConfigRepository<ConfigItem>
{
    
    private readonly AppDbContext _dbContext;

    public ConfigRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConfigItem?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _dbContext.Config.Where(c => c.Key == key).FirstOrDefaultAsync(ct);
    }

    public async Task<ConfigItem?> GetAsync(int id, CancellationToken ct = default)
    {
        var item = await _dbContext.Config.Where(c => c.Id == id).FirstOrDefaultAsync(ct);
        return item;
    }
    
    public async Task<IEnumerable<ConfigItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Config.ToListAsync(ct);
    }

    public async Task AddAsync(ConfigItem item, CancellationToken ct = default)
    {
        _dbContext.Config.Add(item);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ConfigItem item, CancellationToken ct = default)
    {
        _dbContext.Config.Update(item);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _dbContext.Config.Remove(new ConfigItem { Id = id });
        await _dbContext.SaveChangesAsync(ct);
    }
}