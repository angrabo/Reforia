using System.Linq.Expressions;

namespace Reforia.Infrastructure.LiteDb.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    IEnumerable<T> GetAll();
    T GetById(Guid id);
    void Insert(T entity);
    bool Update(T entity);
    bool Delete(Guid id);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
}