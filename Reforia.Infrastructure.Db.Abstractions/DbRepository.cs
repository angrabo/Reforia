using System.Linq.Expressions;
using LiteDB;
using Reforia.Infrastructure.LiteDb.Interfaces;

namespace Reforia.Infrastructure.LiteDb;

public class DbRepository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly ILiteCollection<T> _collection;

    public DbRepository(ILiteDatabase database, string? collectionName = null)
    {
        _collection = database.GetCollection<T>(collectionName ?? typeof(T).Name.ToLower());

        _collection.EnsureIndex(x => x.Id, true);
    }

    public IEnumerable<T> GetAll() => _collection.FindAll();

    public T GetById(Guid id) => _collection.FindById(id);

    public void Insert(T entity)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();
        _collection.Insert(entity);
    }

    public bool Update(T entity) => _collection.Update(entity);

    public bool Delete(Guid id) => _collection.Delete(id);

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => _collection.Find(predicate);
}