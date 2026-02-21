namespace Reforia.Core.Common.Database.Interfaces;

public interface IService<T> where T : class
{
    Task<T?> GetAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T item, CancellationToken ct = default);
    Task UpdateAsync(T item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}