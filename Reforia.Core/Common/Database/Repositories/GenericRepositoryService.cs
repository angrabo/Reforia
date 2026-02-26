using Reforia.Core.Common.Database.Interfaces;

namespace Reforia.Core.Common.Database.Repositories;

public class GenericRepositoryService<T> : IService<T> where T : class
{
    protected readonly IRepository<T> _repository;

    public GenericRepositoryService(IRepository<T> repository)
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