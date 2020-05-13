using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

using PayLend.Database;
using PayLend.Repository.Interface;

namespace PayLend.Repository
{
    public class Repository<TEntity> :IDisposable, IEmailRepository<TEntity> where TEntity : class
    {
        protected PayLendContext Db = new PayLendContext();

        public void Add(TEntity obj)
        {
            Db.Set<TEntity>().Add(obj);
            Db.SaveChanges();
        }

        public void Create<T>(IEnumerable<T> entity) where T : class
        {
            Db.Set<T>().AddRange(entity);
            Db.SaveChanges();
        }

        public T Create<T>(T entity) where T : class
        {
            Db.Set<T>().Add(entity);
            return entity;
        }

        public TEntity GetById(int id)
        {
            return Db.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Db.Set<TEntity>().ToList();
        }

        public void Update(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Modified;
            Db.SaveChanges();
        }

        public void Remove(TEntity obj)
        {
            Db.Set<TEntity>().Remove(obj);
            Db.SaveChanges();
        }

        public void Delete<T>(IQueryable<T> entities) where T : class
        {
            Db.Set<T>().RemoveRange(entities);
            Db.SaveChanges();
        }

        IQueryable<T> IEmailRepository<TEntity>.FindById<T>(Expression<Func<T, bool>> filter, bool noTracking)
        {
            var contextValue = Db.Set<T>().Where(filter);

            if (noTracking)
                contextValue.AsNoTracking();

            return contextValue;
        }

        public void Rollback()
        {
            Db.Rollback();
        }

        void IEmailRepository<TEntity>.Dispose()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(int id) where T : class
        {
            return Db.Set<T>().Find(id);
        }


        public IQueryable<T> GetAll<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null, bool noTracking = true) where T : class
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<T> query = Db.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }
            if (noTracking)
                query.AsNoTracking();

            return query;
        }

        void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveLogChanges()
        {
            LogOperations();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }
        }

        private void LogOperations()
        {
            var cp = (ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            string emailUsuarioLogado = string.Empty;
            var claim = cp.FindFirst("emails");
            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                emailUsuarioLogado = claim.Value;
            }
            else
            {
                emailUsuarioLogado = "PayLendFO";
            }

            var stateAdded = new[] { EntityState.Added };
            var statesModifiedAndDeleted = new[] { EntityState.Modified, EntityState.Deleted };

            var changesToLogAdded = Db.ChangeTracker.Entries().Where(x => stateAdded.Contains(x.State)).ToList();
            var changesToLogModifiedAndDeleted = Db.ChangeTracker.Entries().Where(x => statesModifiedAndDeleted.Contains(x.State)).ToList();

            if (changesToLogModifiedAndDeleted.Any())
            {
                foreach (var change in changesToLogModifiedAndDeleted)
                {
                    LogContext.LogOperation(Db, change, change.State, emailUsuarioLogado);
                }
            }

            if (changesToLogAdded.Any())
            {
                this.Db.SaveChanges();
                foreach (var change in changesToLogAdded)
                {
                    LogContext.LogOperation(Db, change, EntityState.Added, emailUsuarioLogado);
                }
            }
        }

        #region initial repository
        //public Repository(TContext context)
        //{
        //    Context = context;
        //}

        //public T Create<T>(T entity) where T : class
        //{
        //    Context.Set<T>().Add(entity);
        //    return entity;
        //}

        //public void Create<T>(IEnumerable<T> entity) where T : class
        //{
        //    Context.Set<T>().AddRange(entity);
        //}

        //public void Delete<T>(T entity) where T : class
        //{
        //    Context.Set<T>().Remove(entity);
        //}

        //public void Delete<T>(IQueryable<T> entities) where T : class
        //{
        //    Context.Set<T>().RemoveRange(entities);
        //}

        //public T Get<T>(int id) where T : class
        //{
        //    return Context.Set<T>().Find(id);
        //}

        //public IQueryable<T> GetAll<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null, bool noTracking = true) where T : class
        //{
        //    includeProperties = includeProperties ?? string.Empty;
        //    IQueryable<T> query = Context.Set<T>();

        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }

        //    foreach (var includeProperty in includeProperties.Split
        //        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //    {
        //        query = query.Include(includeProperty);
        //    }

        //    if (orderBy != null)
        //    {
        //        query = orderBy(query);
        //    }

        //    if (skip.HasValue)
        //    {
        //        query = query.Skip(skip.Value);
        //    }

        //    if (take.HasValue)
        //    {
        //        query = query.Take(take.Value);
        //    }
        //    if (noTracking)
        //        query.AsNoTracking();

        //    return query;
        //}

        //public T Update<T>(T entity) where T : class
        //{
        //    Context.Set<T>().Attach(entity);
        //    Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        //    return entity;
        //}

        //IQueryable<T> IRepository.FindById<T>(Expression<Func<T, bool>> filter, bool noTracking)
        //{
        //    var contextValue = Context.Set<T>().Where(filter);

        //    if (noTracking)
        //        contextValue.AsNoTracking();

        //    return contextValue;
        //}

        //public void RollBack()
        //{
        //    Context.ChangeTracker.Entries().ToList().ForEach(m =>
        //    {
        //        m.State = EntityState.Detached;
        //    });
        //}

        //public void SaveChanges(bool isLogActive = false)
        //{
        //    if (isLogActive)
        //        LogOperations();


        //    Context.SaveChanges();
        //}

        //public T Unchange<T>(T entity) where T : class
        //{
        //    Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        //    return entity;
        //}
        #endregion

        #region IDisposable Support
        //private bool _disposedValue = false; // To detect redundant calls

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects).
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //        // TODO: set large fields to null.

        //        _disposedValue = true;
        //    }
        //}

        //// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //// ~Repository() {
        ////   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        ////   Dispose(false);
        //// }

        //// This code added to correctly implement the disposable pattern.
        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(true);
        //    // TODO: uncomment the following line if the finalizer is overridden above.
        //    // GC.SuppressFinalize(this);
        //}
        #endregion
    }
}
