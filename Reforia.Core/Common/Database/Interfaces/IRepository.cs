namespace Reforia.Core.Common.Database.Interfaces;

public interface IRepository<T>
{
    public Task<T?> GetAsync(int id, CancellationToken ct = default);
    public Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    public Task AddAsync(T item, CancellationToken ct = default);
    public Task UpdateAsync(T item, CancellationToken ct = default);
    public Task DeleteAsync(int id, CancellationToken ct = default);
}