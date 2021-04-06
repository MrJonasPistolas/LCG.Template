using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LCG.Template.Common.Data.Contracts
{
    public interface IDataRepository : IDisposable
    {
    }

    public interface IDataRepository<T> : IDisposable, IDataRepository where T : class, IIdentifiableEntity, new()
    {
        T Add(T entity);
        IEnumerable<T> Add(IEnumerable<T> entities);
        T Remove(T entity);
        IEnumerable<T> Remove(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Remove(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        T Update(T entity);
        IEnumerable<T> Update(IEnumerable<T> entities);
        IQueryable<T> Get();
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}
