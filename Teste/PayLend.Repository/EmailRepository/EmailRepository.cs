using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using PayLend.Database;

namespace PayLend.Repository.EmailRepository
{
    public class EmailRepository<TContext> : IEmailRepository, IDisposable where TContext : DbContext
    {
        protected readonly TContext context;

        public EmailRepository(TContext _context)
        {
            context = _context;
        }

        public T Create<T>(T entity) where T : class
        {
            context.Set<T>().Add(entity);
            return entity;
        }

        public void Create<T>(IEnumerable<T> entity) where T : class
        {
            context.Set<T>().AddRange(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Set<T>().Remove(entity);
        }

        public void Delete<T>(IQueryable<T> entities) where T : class
        {
            context.Set<T>().RemoveRange(entities);
        }

        public T Get<T>(int id) where T : class
        {
            return context.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null, bool NoTracking = true) where T : class
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<T> query = context.Set<T>();

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
            if (NoTracking)
                query.AsNoTracking();

            return query;
        }

        public T Update<T>(T entity) where T : class
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return entity;
        }

        IQueryable<T> IEmailRepository.FindById<T>(Expression<Func<T, bool>> filter, bool NoTracking)
        {
            var contextValue = context.Set<T>().Where(filter);

            if (NoTracking)
                contextValue.AsNoTracking();

            return contextValue;
        }

        public void RollBack()
        {
            context.ChangeTracker.Entries().ToList().ForEach(m =>
            {
                m.State = EntityState.Detached;
            });
        }

        public void SaveChanges(bool isLogActive = false)
        {
            if (isLogActive)
                LogOperations();


            context.SaveChanges();
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

            var changesToLogAdded = context.ChangeTracker.Entries().Where(x => stateAdded.Contains(x.State)).ToList();
            var changesToLogModifiedAndDeleted = context.ChangeTracker.Entries().Where(x => statesModifiedAndDeleted.Contains(x.State)).ToList();

            if (changesToLogModifiedAndDeleted.Any())
            {
                foreach (var change in changesToLogModifiedAndDeleted)
                {
                    LogContext.LogOperation(context, change, change.State, emailUsuarioLogado);
                }
            }

            if (changesToLogAdded.Any())
            {
                this.context.SaveChanges();
                foreach (var change in changesToLogAdded)
                {
                    LogContext.LogOperation(context, change, EntityState.Added, emailUsuarioLogado);
                }
            }
        }

        public T Unchange<T>(T entity) where T : class
        {
            context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return entity;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
