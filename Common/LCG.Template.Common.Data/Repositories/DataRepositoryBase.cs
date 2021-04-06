using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Data.Extensions;
using LCG.Template.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LCG.Template.Common.Data.Repositories
{
    public abstract class DataRepositoryBase<T, U> : IDisposable,
        IPageable<T>,
        IDataRepository<T> where T : class,
        IIdentifiableEntity, new()
        where U : DbContext
    {
        private U _context = null;

        protected U Context
        {
            get { return _context; }
        }

        public DataRepositoryBase(U context)
        {
            _context = context;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public T Add(T entity)
        {
            T result = AddEntity(Context, entity);
            Context.SaveChanges();
            SetState(result, EntityState.Detached);
            return result;
        }

        protected virtual T AddEntity(U context, T entity)
        {
            SetState(entity, EntityState.Added);
            return entity;
        }

        public IEnumerable<T> Add(IEnumerable<T> entities)
        {
            var results = AddEntities(Context, entities);
            Context.SaveChanges();
            SetState(results, EntityState.Detached);
            return results;
        }

        protected virtual IEnumerable<T> AddEntities(U context, IEnumerable<T> entities)
        {
            SetState(entities, EntityState.Added);
            return entities;
        }

        public IEnumerable<T> Remove(Expression<Func<T, bool>> predicate)
        {
            var results = RemoveEntities(Context, predicate);
            Context.SaveChanges();
            SetState(results, EntityState.Detached);
            return results;
        }

        protected virtual IEnumerable<T> RemoveEntities(U context, Expression<Func<T, bool>> predicate)
        {
            var entities = context.Set<T>().Where(predicate);
            SetState(entities, EntityState.Deleted);
            return entities;
        }

        public T Remove(T entity)
        {
            var result = RemoveEntity(Context, entity);
            Context.SaveChanges();
            SetState(entity, EntityState.Detached);
            return result;
        }

        protected virtual T RemoveEntity(U entityContext, T entity)
        {
            SetState(entity, EntityState.Deleted);
            return entity;
        }

        public IEnumerable<T> Remove(IEnumerable<T> entities)
        {
            var results = RemoveEntities(Context, entities);
            Context.SaveChanges();
            SetState(entities, EntityState.Detached);
            return results;
        }

        protected virtual IEnumerable<T> RemoveEntities(U context, IEnumerable<T> entities)
        {
            SetState(entities, EntityState.Deleted);
            return entities;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var result = UpdateEntity(Context, entity);
            await Context.SaveChangesAsync();
            SetState(entity, EntityState.Detached);
            return result;
        }

        public T Update(T entity)
        {
            var result = UpdateEntity(Context, entity);
            Context.SaveChanges();
            SetState(entity, EntityState.Detached);
            return result;
        }

        protected virtual T UpdateEntity(U context, T entity)
        {
            SetState(entity, EntityState.Modified);
            return entity;
        }

        public IEnumerable<T> Update(IEnumerable<T> entities)
        {
            var results = UpdateEntities(Context, entities);
            Context.SaveChanges();
            SetState(entities, EntityState.Detached);
            return results;
        }

        protected virtual IEnumerable<T> UpdateEntities(U context, IEnumerable<T> entities)
        {
            SetState(entities, EntityState.Modified);
            return entities;
        }

        public IQueryable<T> Query()
        {
            return Context.Set<T>().AsNoTracking();
        }

        public IQueryable<Z> Query<Z>() where Z : class
        {
            return Context.Set<Z>().AsNoTracking();
        }


        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return Query().Where(predicate);
        }

        public IQueryable<Z> Get<Z>() where Z : class
        {
            return Context.Set<Z>().AsNoTracking();
        }

        public IQueryable<Z> Get<Z>(Expression<Func<Z, bool>> predicate) where Z : class
        {
            return Get<Z>().Where(predicate);
        }

        public IQueryable<T> Get()
        {
            var results = GetEntities(Context);
            return results;
        }

        protected virtual IQueryable<T> GetEntities(U context)
        {
            return context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            var results = GetEntities(Context, predicate); ;
            return results;
        }

        protected virtual IQueryable<T> GetEntities(U entityContext, Expression<Func<T, bool>> predicate)
        {
            return entityContext.Set<T>().AsNoTracking().Where(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return FirstOrDefaultEntity(_context, predicate);
        }

        protected virtual T FirstOrDefaultEntity(U entityContext, Expression<Func<T, bool>> predicate)
        {
            return entityContext.Set<T>().AsNoTracking().FirstOrDefault(predicate);
        }

        protected void SetState(IEnumerable<T> entities, EntityState state)
        {
            foreach (var entity in entities)
            {
                SetState(entity, state);
            }
        }

        protected void SetState(T entity, EntityState state)
        {
            Context.Entry(entity).State = state;
        }

        #region IDisposable Members

        ~DataRepositoryBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    // free managed resources
            //    if (_context != null)
            //    {
            //        _context.Dispose();
            //        _context = null;
            //    }
            //}
        }

        #endregion

        #region IPageable Interface
        public IQueryable<T> Get(ref PageOption pageOption)
        {
            return Get((Expression<Func<T, bool>>)null, ref pageOption);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate, ref PageOption pageOption)
        {
            var query = (predicate == null) ? this.Get() : this.Get(predicate);
            return Get(query, ref pageOption);
        }

        public IQueryable<T> Get(IQueryable<T> query, ref PageOption pageOption)
        {
            query = Get<T>(query, ref pageOption);
            return query;
        }

        public IQueryable<Model> Get<Model>(IQueryable<Model> query, ref PageOption pageOption)
        {

            pageOption.Count = query.Count();

            if (pageOption.Sort != null)
            {
                foreach (var sort in pageOption.Sort)
                {
                    if (sort.Dir != null && sort.Dir == "desc")
                    {
                        query = query.OrderBy(sort.Field, OrderByMethod.OrderByDescending);
                    }
                    else
                    {
                        query = query.OrderBy(sort.Field, OrderByMethod.OrderBy);
                    }
                }
            }

            if (pageOption.Filter != null)
            {
                foreach (var filter in pageOption.Filter.Filters)
                {
                    var operators = Operators.Equal;
                    if (filter.Operator.Contains("contains"))
                    {
                        operators = Operators.Contains;
                    }
                    else if (filter.Operator.Contains("gte"))
                    {
                        operators = Operators.GreaterOrEqual;
                    }
                    else if (filter.Operator.Contains("gt"))
                    {
                        operators = Operators.GreaterThan;
                    }
                    else if (filter.Operator.Contains("lte"))
                    {
                        operators = Operators.LowerOrEqual;
                    }
                    else if (filter.Operator.Contains("lt"))
                    {
                        operators = Operators.LessThan;
                    }


                    query = query.Where(filter.Field, (object)filter.Value, operators);
                }
                pageOption.Count = query.Count();
            }

            query = query.Skip(pageOption.Skip).Take(pageOption.Take);
            return query;
        }

        /// <summary>  
        /// Example of usage: 
        ///     int count = 0;
        ///     var test = housingRepository.Get(1, 10, out count);
        ///     test = housingRepository.Get(1, 10, out count, o => o.OrderBy(k => k.RNALId));
        ///     test = housingRepository.Get(1, 10, out count, c=>c.Active==true, o => o.OrderBy(k => k.RNALId));
        ///     test = housingRepository.Get(1, 10, out count, housingRepository.Get(c => c.Active == true), o => o.OrderBy(k => k.RNALId));
        /// </summary>  
        public IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount)
        {
            var query = this.Get();
            return Get(pageNumber, pageSize, out totalDataCount, query);
        }

        public IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            var query = this.Get();
            return Get(pageNumber, pageSize, out totalDataCount, query, orderBy);
        }

        public IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            var query = this.Get(filter);
            return Get(pageNumber, pageSize, out totalDataCount, query, orderBy);
        }

        public IQueryable<T> Get(int pageNumber, int pageSize, out int totalDataCount, IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            totalDataCount = query.Count();
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return query;
        }
        #endregion  
    }
}
