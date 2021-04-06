using LCG.Template.Common.Data.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LCG.Template.Common.Data.Contracts
{
    public interface IPageable<T> where T : class
    {
        IQueryable<T> Get(ref PageOption pageOption);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate, ref PageOption pageOption);
        IQueryable<T> Get(IQueryable<T> query, ref PageOption pageOption);
        IQueryable<Model> Get<Model>(IQueryable<Model> query, ref PageOption pageOption);
        IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount);
        IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy);
        IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
    }
}
