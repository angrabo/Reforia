using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Database.Interfaces;
using System.Linq.Expressions;

namespace Reforia.Core.Common.Database.Repositories;

public class ARepository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T>  _dbSet;

    public ARepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public virtual async Task<T?> GetAsync(int id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync([id], ct);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public virtual async Task AddAsync(T item, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(item, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(T item, CancellationToken ct = default)
    {
        _dbSet.Update(item);
        await _dbContext.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    { 
        var item = await GetAsync(id, ct);
        if (item == null)
            return;
        _dbSet.Remove(item);
        await _dbContext.SaveChangesAsync(ct);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(ct);
    }
}