using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Database.Repositories;

public abstract class ARepositoryService<T> : IService<T> where T : class
{
    private readonly IRepository<T> _repository;

    protected ARepositoryService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public Task<T?> GetAsync(int id, CancellationToken ct = default) =>
        _repository.GetAsync(id, ct);

    public Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) =>
        _repository.GetAllAsync(ct);

    public Task AddAsync(T item, CancellationToken ct = default) =>
        _repository.AddAsync(item, ct);

    public Task UpdateAsync(T item, CancellationToken ct = default) =>
        _repository.UpdateAsync(item, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default) =>
        _repository.DeleteAsync(id, ct);
}